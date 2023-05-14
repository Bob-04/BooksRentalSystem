using System.ComponentModel.DataAnnotations;

namespace BooksRentalSystem.Snapshotting.MongoMemory.DbModels;

public class AggregateSnapshotSqlModel
{
    [Key] public Guid AggregateKey { get; set; }
    public long Version { get; set; }
    public string Payload { get; set; }
}