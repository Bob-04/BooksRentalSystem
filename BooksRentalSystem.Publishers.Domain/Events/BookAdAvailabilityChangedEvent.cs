using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Publishers.Domain.Events;

public record BookAdAvailabilityChangedEvent : Event
{
    public Guid Id { get; init; }
    public bool IsAvailable { get; init; }
}