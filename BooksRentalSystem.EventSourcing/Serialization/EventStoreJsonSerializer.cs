using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace BooksRentalSystem.EventSourcing.Serialization;

public interface IEventStoreJsonSerializer
{
    public string Serialize<T>(T data);

    public object? Deserialize(string data, Type type);
}

public class EventStoreJsonSerializer : IEventStoreJsonSerializer
{
    private readonly JsonSerializerSettings _jsonSerializerSettings;
    private readonly JsonSerializer _jsonSerializer;

    public EventStoreJsonSerializer()
    {
        _jsonSerializerSettings = new JsonSerializerSettings
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
        _jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
    }

    public string Serialize<T>(T data)
    {
        TextWriter stringWriter = new StringWriter();
        _jsonSerializer.Serialize(stringWriter, data);

        return stringWriter.ToString() ?? string.Empty;
    }

    public object? Deserialize(string data, Type type)
    {
        return JsonConvert.DeserializeObject(data, type, _jsonSerializerSettings);
    }
}