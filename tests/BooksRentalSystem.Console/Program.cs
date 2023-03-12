using Anabasis.InMemory.SnapshotStores;
using BooksRentalSystem.Console.Domain.Aggregates;
using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.EventSourcing.Serialization;
using EventStore.Client;

const string esConnectionString = "esdb://localhost:2115?tls=false";

var inMemorySnapshotStore = new InMemorySnapshotStore();

IEventStoreAggregateRepository repository = new EventStoreAggregateRepository(
    new EventStoreClient(EventStoreClientSettings.Create(esConnectionString)),
    inMemorySnapshotStore,
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