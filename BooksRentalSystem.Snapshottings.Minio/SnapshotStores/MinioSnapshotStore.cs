using System.Reactive.Linq;
using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Snapshotting;
using Minio;
using Minio.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BooksRentalSystem.Snapshottings.Minio.SnapshotStores;

public class MinioSnapshotStore : ISnapshotStore
{
    private static JsonSerializerSettings _jsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        ContractResolver = new CamelCasePropertyNamesContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Converters = new List<JsonConverter>
        {
            new StringEnumConverter()
        },
        TypeNameHandling = TypeNameHandling.Auto
    };

    private readonly IMinioClient _minioClient;

    public MinioSnapshotStore(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new()
    {
        var aggregateItems = await _minioClient.ListObjectsAsync(
            new ListObjectsArgs()
                .WithBucket("snapshots")
        ).ToList();

        var aggregates = new List<TAggregate>();

        foreach (var aggregateItem in aggregateItems)
        {
            TAggregate? aggregateSnapshot = default;

            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket("snapshots")
                    .WithObject(aggregateItem.Key)
                    .WithCallbackStream(stream => aggregateSnapshot = Deserialize<TAggregate>(stream))
            );

            if (aggregateSnapshot != default)
                aggregates.Add(aggregateSnapshot);
        }

        return aggregates.ToArray();
    }

    public async Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, long? version = null)
        where TAggregate : Aggregate, new()
    {
        TAggregate? aggregateSnapshot = default;

        try
        {
            await _minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket("snapshots")
                    .WithObject($"{typeof(TAggregate).Name}/{streamId}.json")
                    .WithCallbackStream(stream => aggregateSnapshot = Deserialize<TAggregate>(stream))
            );
        }
        catch (ObjectNotFoundException)
        {
            return default;
        }

        return aggregateSnapshot;
    }

    public async Task Save<TAggregate>(TAggregate aggregate)
        where TAggregate : Aggregate, new()
    {
        var aggregateStream =
            ToStream(JsonConvert.SerializeObject(aggregate, Formatting.Indented, _jsonSerializerSettings));

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket("snapshots")
                .WithObject($"{typeof(TAggregate).Name}/{aggregate.Id}.json")
                .WithStreamData(aggregateStream)
                .WithObjectSize(aggregateStream.Length)
                .WithContentType("application/json")
        );
    }

    private static T? Deserialize<T>(Stream s)
    {
        using var reader = new StreamReader(s);
        using var jsonReader = new JsonTextReader(reader);
        var ser = new JsonSerializer();
        return ser.Deserialize<T>(jsonReader);
    }

    private static Stream ToStream(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}