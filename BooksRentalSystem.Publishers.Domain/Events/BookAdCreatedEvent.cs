using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.Publishers.Domain.Enums;

namespace BooksRentalSystem.Publishers.Domain.Events;

public record BookAdCreatedEvent : Event
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string ImageUrl { get; init; }
    public decimal PricePerDay { get; init; }
    public int? PagesNumber { get; init; }
    public string Language { get; init; }
    public DateTime? PublicationDate { get; init; }
    public CoverType? CoverType { get; init; }
    public int PublisherId { get; init; }
    public string AuthorName { get; init; }
    public int CategoryId { get; init; }
}