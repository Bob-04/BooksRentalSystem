namespace BooksRentalSystem.EventSourcing.Events;

public record EventMetadata
{
    public string? EventFullName { get; init; }
}