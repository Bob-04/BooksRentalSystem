using Anabasis.Common;
using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Snapshotting;
using Microsoft.EntityFrameworkCore;

namespace Anabasis.InMemory.SnapshotStores;

public class InMemorySnapshotStore : ISnapshotStore
{
    private readonly DbContextOptions<AggregateSnapshotContext> _entityFrameworkOptions;

    class AggregateSnapshotContext : DbContext
    {
        public AggregateSnapshotContext(DbContextOptions<AggregateSnapshotContext> options)
            : base(options)
        {
        }

#nullable disable
        public DbSet<AggregateSnapshot> AggregateSnapshots { get; set; }
#nullable enable

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregateSnapshot>().HasKey(aggregateSnapshot => new
            {
                aggregateSnapshot.EntityId,
                aggregateSnapshot.EventFilter,
                aggregateSnapshot.Version
            });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var entries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is AggregateSnapshot aggregateSnapshot)
                {
                    aggregateSnapshot.LastModifiedUtc = now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }

    public InMemorySnapshotStore()
    {
        _entityFrameworkOptions = new DbContextOptionsBuilder<AggregateSnapshotContext>()
            .UseInMemoryDatabase(databaseName: "AggregateSnapshots")
            .Options;
    }

    public async Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, string[] eventFilters,
        int? version = null)
        where TAggregate : Aggregate, new()
    {

        using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        var eventFilter = string.Concat(eventFilters);

        var aggregateSnapshotQueryable =
            context.AggregateSnapshots.AsQueryable().OrderByDescending(p => p.LastModifiedUtc);

        AggregateSnapshot? aggregateSnapshot = null;

        if (null == version)
        {
            aggregateSnapshot = await aggregateSnapshotQueryable.OrderByDescending(snapshot => snapshot.LastModifiedUtc)
                .FirstOrDefaultAsync(snapshot => snapshot.EntityId == streamId && snapshot.EventFilter == eventFilter);
        }
        else
        {
            aggregateSnapshot = await aggregateSnapshotQueryable.OrderByDescending(snapshot => snapshot.LastModifiedUtc)
                .FirstOrDefaultAsync(snapshot =>
                    snapshot.Version == version && snapshot.EntityId == streamId &&
                    snapshot.EventFilter == eventFilter);
        }


        if (null == aggregateSnapshot) return null;

        var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

        return aggregate;
    }

    public async Task<TAggregate[]> GetByVersionOrLast<TAggregate>(string[] eventFilters, int? version = null)
        where TAggregate : Aggregate, new()
    {
        using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        var eventFilter = string.Concat(eventFilters);

        var isLatest = version == null;

        AggregateSnapshot[]? aggregateSnapshots = null;

        if (isLatest)
        {

            var orderByDescendingQueryable = context.AggregateSnapshots.AsQueryable()
                .OrderByDescending(snapshot => snapshot.LastModifiedUtc);

            //https://github.com/dotnet/efcore/issues/13805
            aggregateSnapshots = await context.AggregateSnapshots.AsQueryable()
                .Where(snapshot => snapshot.EventFilter == eventFilter)
                .OrderByDescending(snapshot => snapshot.LastModifiedUtc)
                .Select(snapshot => snapshot.EntityId)
                .Distinct()
                .SelectMany(snapshot => orderByDescendingQueryable.Where(b => b.EntityId == snapshot).Take(1),
                    (streamId, aggregateSnapshot) => aggregateSnapshot)
                .ToArrayAsync();
        }
        else
        {
            aggregateSnapshots = await context.AggregateSnapshots.AsQueryable()
                .Where(snapshot => snapshot.EventFilter == eventFilter && snapshot.Version == version).ToArrayAsync();
        }

        if (aggregateSnapshots.Length == 0) return new TAggregate[0];

        return aggregateSnapshots
            .Select(aggregateSnapshot => aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>()).ToArray();
    }

    public async Task Save<TAggregate>(string?[] eventFilters, TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        var aggregateSnapshot = new AggregateSnapshot(
            aggregate.Id.ToString(),
            string.Concat(eventFilters),
            aggregate.Version,
            aggregate.ToJson(),
            DateTime.UtcNow
        );

        if (context.AggregateSnapshots.Contains(aggregateSnapshot))
            return;

        context.AggregateSnapshots.Add(aggregateSnapshot);

        await context.SaveChangesAsync();
    }

    public async Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new()
    {
        var results = new List<TAggregate>();

        using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        foreach (var aggregateSnapshot in await context.AggregateSnapshots.AsQueryable().ToListAsync())
        {
            var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

            results.Add(aggregate);
        }

        return results.ToArray();
    }
}