using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Identity.Domain.Events;

public record UserUpdatedEvent : Event
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string PhoneNumber { get; init; }
}