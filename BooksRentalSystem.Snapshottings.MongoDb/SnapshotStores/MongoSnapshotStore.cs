using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Snapshotting;
using BooksRentalSystem.Snapshottings.MongoDb.DbModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace BooksRentalSystem.Snapshottings.MongoDb.SnapshotStores;

public class MongoSnapshotStore : ISnapshotStore
{
    private readonly IMongoCollection<AggregateSnapshot> _snapshotsCollection;

    public MongoSnapshotStore(string mongoDbConnectionString)
    {
        var mongoClient = new MongoClient(mongoDbConnectionString);
        var mongoDatabase = mongoClient.GetDatabase("Snapshots");
        _snapshotsCollection = mongoDatabase.GetCollection<AggregateSnapshot>("AggregateSnapshots");
    }

    public async Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, long? version = null)
        where TAggregate : Aggregate, new()
    {
        var aggregateSnapshot = await _snapshotsCollection
            .Find(a => a.AggregateKey == streamId && (version == default || a.Version == version))
            .SortByDescending(a => a.Version)
            .FirstOrDefaultAsync();

        if (aggregateSnapshot == default) return default;

        var aggregate = BsonSerializer.Deserialize<TAggregate>(aggregateSnapshot.Payload);
        return aggregate;
    }

    public async Task Save<TAggregate>(TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        var existingSnapshot = await GetByVersionOrLast<TAggregate>(aggregate.Id.ToString(), aggregate.Version);
        if (existingSnapshot != default) return;

        var aggregateSnapshot = new AggregateSnapshot
        {
            AggregateKey = aggregate.Id.ToString(),
            Version = aggregate.Version,
            Payload = aggregate.ToBsonDocument()
        };

        await _snapshotsCollection.InsertOneAsync(aggregateSnapshot);
    }

    public async Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new()
    {
        var results = new List<TAggregate>();

        foreach (var aggregateSnapshot in await _snapshotsCollection.Find(_ => true).ToListAsync())
        {
            var aggregate = BsonSerializer.Deserialize<TAggregate>(aggregateSnapshot.Payload);

            results.Add(aggregate);
        }

        return results.ToArray();
    }
}