using BooksRentalSystem.EventSourcing.Aggregates;

namespace BooksRentalSystem.EventSourcing.Snapshotting;

public interface ISnapshotStore
{
    Task<TAggregate[]> GetAll<TAggregate>()
        where TAggregate : Aggregate, new();

    Task<TAggregate[]> GetByVersionOrLast<TAggregate>(string[] eventFilters, int? version = null)
        where TAggregate : Aggregate, new();

    Task<TAggregate?> GetByVersionOrLast<TAggregate>(string streamId, string[] eventFilters, int? version = null)
        where TAggregate : Aggregate, new();

    Task Save<TAggregate>(string?[] eventFilters, TAggregate aggregate)
        where TAggregate : Aggregate, new();
}