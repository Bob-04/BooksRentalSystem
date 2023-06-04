using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Publishers.Domain.Events;

public record BookAdViewedEvent : Event
{
    public Guid Id { get; init; }
    public string ViewedBy { get; init; }
}