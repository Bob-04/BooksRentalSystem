using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Serialization;
using BooksRentalSystem.EventSourcing.Snapshotting;
using BooksRentalSystem.Snapshotting.MongoMemory.DbContexts;
using BooksRentalSystem.Snapshotting.MongoMemory.DbModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BooksRentalSystem.Snapshotting.MongoMemory.SnapshotStores;

public class MongoMemorySnapshotStore : ISnapshotStore
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly DbContextOptions<AggregateSnapshotInMemoryContext> _inMemoryContextOptions;
    private readonly IEventStoreJsonSerializer _jsonSerializer;

    public MongoMemorySnapshotStore(string mongoDbConnectionString, IEventStoreJsonSerializer jsonSerializer)
    {
        var mongoClient = new MongoClient(mongoDbConnectionString);
        _mongoDatabase = mongoClient.GetDatabase("Snapshots");
        _inMemoryContextOptions = new DbContextOptionsBuilder<AggregateSnapshotInMemoryContext>()
            .UseInMemoryDatabase(databaseName: "Snapshots")
            .Options;
        _jsonSerializer = jsonSerializer;
    }

    public async Task<TAggregate> GetByVersionOrLast<TAggregate>(string streamId, long? version = null)
        where TAggregate : Aggregate, new()
    {
        await using var inMemoryContext = new AggregateSnapshotInMemoryContext(_inMemoryContextOptions);

        var inMemorySnapshot = await inMemoryContext.AggregateSnapshots
            .Where(a => a.AggregateKey == Guid.Parse(streamId) && (version == default || a.Version == version))
            .OrderByDescending(a => a.Version)
            .FirstOrDefaultAsync();
        if (inMemorySnapshot != default)
        {
            var inMemoryAggregate =
                _jsonSerializer.Deserialize(inMemorySnapshot.Payload, typeof(TAggregate)) as TAggregate;
            return inMemoryAggregate;
        }

        var mongoSnapshotsCollection = _mongoDatabase.GetCollection<AggregateSnapshot>(typeof(TAggregate).Name);

        var mongoSnapshot = await mongoSnapshotsCollection
            .Find(a => a.AggregateKey == streamId && (version == default || a.Version == version))
            .SortByDescending(a => a.Version)
            .FirstOrDefaultAsync();

        if (mongoSnapshot == default) return default;

        var mongoAggregate = BsonSerializer.Deserialize<TAggregate>(mongoSnapshot.Payload);
        return mongoAggregate;
    }

    public async Task Save<TAggregate>(TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        var snapshotsCollection = _mongoDatabase.GetCollection<AggregateSnapshot>(typeof(TAggregate).Name);

        var existingSnapshot = await GetByVersionOrLast<TAggregate>(aggregate.Id.ToString(), aggregate.Version);
        if (existingSnapshot != default) return;

        var mongoSnapshot = new AggregateSnapshot
        {
            AggregateKey = aggregate.Id.ToString(),
            Version = aggregate.Version,
            Payload = aggregate.ToBsonDocument()
        };

        await snapshotsCollection.InsertOneAsync(mongoSnapshot);

        var memorySnapshot = new AggregateSnapshotSqlModel
        {
            AggregateKey = aggregate.Id,
            Version = aggregate.Version,
            Payload = _jsonSerializer.Serialize(aggregate)
        };

        await using var inMemoryContext = new AggregateSnapshotInMemoryContext(_inMemoryContextOptions);
        inMemoryContext.AggregateSnapshots.Add(memorySnapshot);
        await inMemoryContext.SaveChangesAsync();
    }

    public async Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new()
    {
        var snapshotsCollection = _mongoDatabase.GetCollection<AggregateSnapshot>(typeof(TAggregate).Name);

        var results = new List<TAggregate>();

        foreach (var aggregateSnapshot in await snapshotsCollection.Find(_ => true).ToListAsync())
        {
            var aggregate = BsonSerializer.Deserialize<TAggregate>(aggregateSnapshot.Payload);

            results.Add(aggregate);
        }

        return results.ToArray();
    }
}