using Anabasis.InMemory.SnapshotStores;
using Anabasis.SqlServer.SnapshotStores;
using BooksRentalSystem.Console.Domain.Aggregates;
using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.EventSourcing.Serialization;
using BooksRentalSystem.Snapshottings.MongoDb.SnapshotStores;
using EventStore.Client;

const string esConnectionString = "esdb://localhost:2115?tls=false";
const string sqlConnectionString =
    "Server=localhost,1434;Database=BooksRentalSnapshots;User Id=sa; Password=yourStrongPassword12!@;MultipleActiveResultSets=true;TrustServerCertificate=True;";
const string mongoDbConnectionString = "mongodb://localhost:27017";

var inMemorySnapshotStore = new InMemorySnapshotStore();
var sqlSnapshotStore = new SqlSnapshotStore(sqlConnectionString);
var mongoDbSnapshotStore = new MongoSnapshotStore(mongoDbConnectionString);

IEventStoreAggregateRepository repository = new EventStoreAggregateRepository(
    new EventStoreClient(EventStoreClientSettings.Create(esConnectionString)),
    mongoDbSnapshotStore,
    new EventStoreJsonSerializer()
);

var aggregateId = Guid.NewGuid();

var aggregate = new TestCounterAggregate { Id = aggregateId };
for (var i = 0; i < 100; i++)
    aggregate.IncreaseCounter();
for (var i = 0; i < 100; i++)
    aggregate.DecreaseCounter();

await repository.SaveAsync(aggregate);

var aggregate1 = await repository.LoadAsync<TestCounterAggregate>(aggregateId);
for (var i = 0; i < 200; i++)
    aggregate1.IncreaseCounter();

await repository.SaveAsync(aggregate1);

var aggregate2 = await repository.LoadAsync<TestCounterAggregate>(aggregateId);

Console.WriteLine("Finished");