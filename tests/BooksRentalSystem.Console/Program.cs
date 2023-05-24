using System.Diagnostics;
using Anabasis.InMemory.SnapshotStores;
using Anabasis.SqlServer.SnapshotStores;
using BooksRentalSystem.Console.Domain.Aggregates;
using BooksRentalSystem.Console.Domain.Enums;
using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.EventSourcing.Serialization;
using BooksRentalSystem.Snapshotting.MongoMemory.SnapshotStores;
using BooksRentalSystem.Snapshottings.Minio.Options;
using BooksRentalSystem.Snapshottings.Minio.SnapshotStores;
using BooksRentalSystem.Snapshottings.MongoDb.SnapshotStores;
using EventStore.Client;
using Minio;

const string esConnectionString = "esdb://localhost:2115?tls=false";
const string sqlConnectionString =
    "Server=localhost,1434;Database=BooksRentalSnapshots;User Id=sa; Password=yourStrongPassword12!@;MultipleActiveResultSets=true;TrustServerCertificate=True;";
const string mongoDbConnectionString = "mongodb://localhost:27018";
var minioOptions = new MinioOptions
{
    Endpoint = "localhost:9000",
    AccessKey = "root",
    SecretKey = "minioStrongPassword12!@"
};

var inMemorySnapshotStore = new InMemorySnapshotStore();
var sqlSnapshotStore = new SqlSnapshotStore(sqlConnectionString);
var mongoDbSnapshotStore = new MongoSnapshotStore(mongoDbConnectionString);
var minioSnapshotStore = new MinioSnapshotStore(
    new MinioClient()
        .WithEndpoint(minioOptions.Endpoint)
        .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
        .Build()
);
var mongoMemorySnapshotStore = new MongoMemorySnapshotStore(
    mongoDbConnectionString,
    true
);

IEventStoreAggregateRepository repository = new EventStoreAggregateRepository(
    new EventStoreClient(EventStoreClientSettings.Create(esConnectionString)),
    mongoMemorySnapshotStore,
    new EventStoreJsonSerializer(),
    true
);

var users = new[] { "Bob", "Peter", "John", "Suzy", "Angel", "Steve", "Andrew", "Ivan" };

var aggregateId = Guid.Parse("00000000-0000-0000-0000-000000000001");

var aggregate = new CalculatorAggregate { Id = aggregateId };

// for (int j = 0; j < 16380; j++)
// {
//     aggregate = new CalculatorAggregate { Id = Guid.NewGuid() };
//     for (var i = 0; i < Random.Shared.Next(100, 600); i++)
//     {
//         if (Random.Shared.Next(100) < 95)
//             aggregate.MakeOperation(
//                 RandomOperation(),
//                 Random.Shared.NextDouble() * 1000,
//                 users[Random.Shared.Next(users.Length)]
//             );
//         else
//             aggregate.Clear(
//                 users[Random.Shared.Next(users.Length)]
//             );
//     }
//
//     await repository.SaveAsync(aggregate);
// }

var aggregateIds = new List<Guid>
{
    Guid.Parse("00000000-0000-0001-0006-000000002000"),
    Guid.Parse("00000000-0000-0001-0006-000000002001"),
    Guid.Parse("00000000-0000-0001-0006-000000002002"),
    Guid.Parse("00000000-0000-0001-0006-000000002003"),
    Guid.Parse("00000000-0000-0001-0006-000000002004"),
    Guid.Parse("00000000-0000-0001-0006-000000002005"),
    Guid.Parse("00000000-0000-0001-0006-000000002006"),
    Guid.Parse("00000000-0000-0001-0006-000000002007"),
    Guid.Parse("00000000-0000-0001-0006-000000002008"),
    Guid.Parse("00000000-0000-0001-0006-000000002009"),
    Guid.Parse("00000000-0000-0001-0006-000000002010")
};

var avg = TimeSpan.Zero;
var first = TimeSpan.Zero;
foreach (var aggrId in aggregateIds)
{
    aggregate = new CalculatorAggregate { Id = aggrId };
    for (var i = 0; i < 5005; i++)
    {
        if (Random.Shared.Next(100) < 95)
            aggregate.MakeOperation(
                RandomOperation(),
                Random.Shared.NextDouble() * 1000,
                users[Random.Shared.Next(users.Length)]
            );
        else
            aggregate.Clear(
                users[Random.Shared.Next(users.Length)]
            );
    }

    var s = Stopwatch.StartNew();
    await repository.SaveAsync(aggregate);
    s.Stop();
    avg += s.Elapsed;
    if (first == TimeSpan.Zero) first = s.Elapsed;
    // Console.WriteLine("Saving: " + s.Elapsed);
}

Console.WriteLine($"Avg saving: {avg / aggregateIds.Count}");
Console.WriteLine($"Avg saving (2+): {(avg - first) / (aggregateIds.Count - 1)}");

avg = TimeSpan.Zero;
first = TimeSpan.Zero;
foreach (var aggrId in aggregateIds)
{
    var s = Stopwatch.StartNew();
    var aggr = await repository.LoadAsync<CalculatorAggregate>(aggrId);
    s.Stop();
    avg += s.Elapsed;
    if (first == TimeSpan.Zero) first = s.Elapsed;
    // Console.WriteLine("Loading: " + s.Elapsed);
}

Console.WriteLine($"Avg loading: {avg / aggregateIds.Count}");
Console.WriteLine($"Avg loading (2+): {(avg - first) / (aggregateIds.Count - 1)}");

//
// var aggregate1 = await repository.LoadAsync<CalculatorAggregate>(aggregateId);
//
// await repository.SaveAsync(aggregate1);
//
// var aggregate2 = await repository.LoadAsync<CalculatorAggregate>(aggregateId);

Console.WriteLine("Finished");



Operation RandomOperation()
{
    var values = System.Enum.GetValues(typeof(Operation));
    return (Operation)values.GetValue(Random.Shared.Next(values.Length))!;
}