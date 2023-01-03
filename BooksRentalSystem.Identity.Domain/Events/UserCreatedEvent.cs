using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Identity.Domain.Events;

public record UserCreatedEvent : Event
{
    public string Email { get; init; }
}