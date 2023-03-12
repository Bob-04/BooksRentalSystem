using Anabasis.Common;
using Anabasis.SqlServer.DbContexts;
using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Snapshotting;
using Microsoft.EntityFrameworkCore;

namespace Anabasis.SqlServer.SnapshotStores;

public class SqlSnapshotStore : ISnapshotStore
{
    private readonly DbContextOptions<AggregateSnapshotDbContext> _dbContextOptions;

    public SqlSnapshotStore(string sqlConnectionString)
    {
        _dbContextOptions = new DbContextOptionsBuilder<AggregateSnapshotDbContext>()
            .UseSqlServer(sqlConnectionString, o => o.EnableRetryOnFailure())
            .Options;
    }

    public async Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, string[] eventFilters,
        int? version = null)
        where TAggregate : Aggregate, new()
    {
        using var context = new AggregateSnapshotDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

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

        if (null == aggregateSnapshot) return default;

        var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

        return aggregate;

    }

    public async Task<TAggregate[]> GetByVersionOrLast<TAggregate>(string[] eventFilters, int? version = null)
        where TAggregate : Aggregate, new()
    {
        using var context = new AggregateSnapshotDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

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

        if (aggregateSnapshots.Length == 0) return System.Array.Empty<TAggregate>();

        return aggregateSnapshots
            .Select(aggregateSnapshot => aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>()).ToArray();

    }

    public async Task Save<TAggregate>(string?[] eventFilters, TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        using var context = new AggregateSnapshotDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

        var aggregateSnapshot = new AggregateSnapshot
        {
            EntityId = aggregate.Id.ToString(),
            Version = aggregate.Version,
            EventFilter = string.Concat(eventFilters),
            SerializedAggregate = aggregate.ToJson(),
        };

        if (await context.AggregateSnapshots.ContainsAsync(aggregateSnapshot)) return;

        context.AggregateSnapshots.Add(aggregateSnapshot);

        await context.SaveChangesAsync();

    }

    public async Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new()
    {
        var results = new List<TAggregate>();

        using var context = new AggregateSnapshotDbContext(_dbContextOptions);
        await context.Database.EnsureCreatedAsync();

        foreach (var aggregateSnapshot in await context.AggregateSnapshots.AsQueryable().ToListAsync())
        {
            var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

            results.Add(aggregate);
        }

        return results.ToArray();

    }
}