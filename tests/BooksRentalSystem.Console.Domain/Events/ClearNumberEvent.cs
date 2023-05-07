using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Console.Domain.Events;

public record ClearNumberEvent : Event
{
    public string MadeBy { get; init; }
}