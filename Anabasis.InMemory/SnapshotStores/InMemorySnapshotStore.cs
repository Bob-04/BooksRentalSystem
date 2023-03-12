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

    public async Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, long? version = null)
        where TAggregate : Aggregate, new()
    {
        await using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        var aggregateSnapshotQueryable =
            context.AggregateSnapshots.AsQueryable().OrderByDescending(p => p.LastModifiedUtc);

        AggregateSnapshot? aggregateSnapshot = null;

        if (null == version)
        {
            aggregateSnapshot = await aggregateSnapshotQueryable.OrderByDescending(snapshot => snapshot.LastModifiedUtc)
                .FirstOrDefaultAsync(snapshot => snapshot.EntityId == streamId);
        }
        else
        {
            aggregateSnapshot = await aggregateSnapshotQueryable.OrderByDescending(snapshot => snapshot.LastModifiedUtc)
                .FirstOrDefaultAsync(snapshot => snapshot.Version == version && snapshot.EntityId == streamId);
        }

        if (null == aggregateSnapshot) return null;

        var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

        return aggregate;
    }

    public async Task Save<TAggregate>(TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        await using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        var aggregateSnapshot = new AggregateSnapshot(
            aggregate.Id.ToString(),
            "",
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

        await using var context = new AggregateSnapshotContext(_entityFrameworkOptions);

        foreach (var aggregateSnapshot in await context.AggregateSnapshots.AsQueryable().ToListAsync())
        {
            var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

            results.Add(aggregate);
        }

        return results.ToArray();
    }
}