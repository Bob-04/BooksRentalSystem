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
    new EventStoreJsonSerializer(),
    true
);

IEventStoreAggregateRepository repository = new EventStoreAggregateRepository(
    new EventStoreClient(EventStoreClientSettings.Create(esConnectionString)),
    mongoMemorySnapshotStore,
    new EventStoreJsonSerializer(),
    true
);

var users = new[] { "Bob", "Peter", "John", "Suzy", "Angel", "Steve", "Andrew", "Ivan" };

const int eventsNumber = 10005;
var aggregateId = Guid.Parse("00000000-0000-0011-0000-000000000032");

// var aggregate = new CalculatorAggregate { Id = aggregateId };

var avgS = TimeSpan.Zero;
var avgL = TimeSpan.Zero;
var firstS = TimeSpan.Zero;
var firstL = TimeSpan.Zero;

for (var i = 0; i < eventsNumber; i++)
{
    var s = Stopwatch.StartNew();
    var aggregate = i == 0
        ? new CalculatorAggregate { Id = aggregateId }
        : await repository.LoadAsync<CalculatorAggregate>(aggregateId);
    s.Stop();
    avgL += s.Elapsed;
    if (firstL == TimeSpan.Zero) firstL = s.Elapsed;

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
    s = Stopwatch.StartNew();
    await repository.SaveAsync(aggregate);
    s.Stop();
    avgS += s.Elapsed;
    if (firstS == TimeSpan.Zero) firstS = s.Elapsed;

    if (i > 0 && i % 100 == 0)
    {
        Console.WriteLine($"{i}: Avg loading: {avgL / (i + 1)}");
        Console.WriteLine($"{i}: Avg saving: {avgS / (i + 1)}");
    }
}

Console.WriteLine($"Avg saving: {avgS / eventsNumber}");
Console.WriteLine($"Avg saving (2+): {(avgS - firstS) / (eventsNumber - 1)}");

Console.WriteLine($"Avg loading: {avgL / eventsNumber}");
Console.WriteLine($"Avg loading (2+): {(avgL - firstL) / (eventsNumber - 1)}");

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

// var aggregateIds = new List<Guid>
// {
//     Guid.Parse("00000000-0000-0001-0006-000000002000"),
//     Guid.Parse("00000000-0000-0001-0006-000000002001"),
//     Guid.Parse("00000000-0000-0001-0006-000000002002"),
//     Guid.Parse("00000000-0000-0001-0006-000000002003"),
//     Guid.Parse("00000000-0000-0001-0006-000000002004"),
//     Guid.Parse("00000000-0000-0001-0006-000000002005"),
//     Guid.Parse("00000000-0000-0001-0006-000000002006"),
//     Guid.Parse("00000000-0000-0001-0006-000000002007"),
//     Guid.Parse("00000000-0000-0001-0006-000000002008"),
//     Guid.Parse("00000000-0000-0001-0006-000000002009"),
//     Guid.Parse("00000000-0000-0001-0006-000000002010")
// };
//
// var avg = TimeSpan.Zero;
// var first = TimeSpan.Zero;
// foreach (var aggrId in aggregateIds)
// {
//     aggregate = new CalculatorAggregate { Id = aggrId };
//     for (var i = 0; i < 5005; i++)
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
//     var s = Stopwatch.StartNew();
//     await repository.SaveAsync(aggregate);
//     s.Stop();
//     avg += s.Elapsed;
//     if (first == TimeSpan.Zero) first = s.Elapsed;
//     // Console.WriteLine("Saving: " + s.Elapsed);
// }
//
// Console.WriteLine($"Avg saving: {avg / aggregateIds.Count}");
// Console.WriteLine($"Avg saving (2+): {(avg - first) / (aggregateIds.Count - 1)}");
//
// avg = TimeSpan.Zero;
// first = TimeSpan.Zero;
// foreach (var aggrId in aggregateIds)
// {
//     var s = Stopwatch.StartNew();
//     var aggr = await repository.LoadAsync<CalculatorAggregate>(aggrId);
//     s.Stop();
//     avg += s.Elapsed;
//     if (first == TimeSpan.Zero) first = s.Elapsed;
//     // Console.WriteLine("Loading: " + s.Elapsed);
// }
//
// Console.WriteLine($"Avg loading: {avg / aggregateIds.Count}");
// Console.WriteLine($"Avg loading (2+): {(avg - first) / (aggregateIds.Count - 1)}");


Console.WriteLine("Finished");



Operation RandomOperation()
{
    var values = System.Enum.GetValues(typeof(Operation));
    return (Operation)values.GetValue(Random.Shared.Next(values.Length))!;
}