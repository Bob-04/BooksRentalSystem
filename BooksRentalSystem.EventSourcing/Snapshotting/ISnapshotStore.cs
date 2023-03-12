using BooksRentalSystem.EventSourcing.Aggregates;

namespace BooksRentalSystem.EventSourcing.Snapshotting;

public interface ISnapshotStore
{
    Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new();

    Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, long? version = null)
        where TAggregate : Aggregate, new();

    Task Save<TAggregate>(TAggregate aggregate)
        where TAggregate : Aggregate, new();
}