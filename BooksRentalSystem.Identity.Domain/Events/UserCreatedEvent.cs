using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Identity.Domain.Events;

public record UserCreatedEvent : Event
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
}