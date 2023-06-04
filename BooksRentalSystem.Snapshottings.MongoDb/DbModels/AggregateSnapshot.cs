using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BooksRentalSystem.Snapshottings.MongoDb.DbModels;

public class AggregateSnapshot
{
    [BsonId]
    [BsonIgnoreIfDefault]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; }

    public string AggregateKey { get; set; }
    public long Version { get; set; }
    public BsonDocument Payload { get; set; }
}