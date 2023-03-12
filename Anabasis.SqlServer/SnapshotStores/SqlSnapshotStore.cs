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

        using var context = new AggregateSnapshotDbContext(_dbContextOptions);
        context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
    }

    public async Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, long? version = null)
        where TAggregate : Aggregate, new()
    {
        await using var context = new AggregateSnapshotDbContext(_dbContextOptions);

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

        if (null == aggregateSnapshot) return default;

        var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

        return aggregate;
    }

    public async Task Save<TAggregate>(TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        await using var context = new AggregateSnapshotDbContext(_dbContextOptions);

        var aggregateSnapshot = new AggregateSnapshot
        {
            EntityId = aggregate.Id.ToString(),
            Version = aggregate.Version,
            EventFilter = "",
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

        await using var context = new AggregateSnapshotDbContext(_dbContextOptions);

        foreach (var aggregateSnapshot in await context.AggregateSnapshots.AsQueryable().ToListAsync())
        {
            var aggregate = aggregateSnapshot.SerializedAggregate.JsonTo<TAggregate>();

            results.Add(aggregate);
        }

        return results.ToArray();
    }
}