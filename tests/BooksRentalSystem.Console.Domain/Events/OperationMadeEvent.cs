using BooksRentalSystem.Console.Domain.Enums;
using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Console.Domain.Events;

public record OperationMadeEvent : Event
{
    public Operation Operation { get; init; }
    public double Number { get; init; }
    public string MadeBy { get; init; }
}