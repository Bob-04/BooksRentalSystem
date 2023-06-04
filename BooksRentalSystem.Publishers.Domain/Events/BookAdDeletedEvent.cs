using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Publishers.Domain.Events;

public record BookAdDeletedEvent : Event
{
    public Guid Id { get; init; }
}