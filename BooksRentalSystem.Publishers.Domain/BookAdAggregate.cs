using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.Publishers.Domain.Enums;
using BooksRentalSystem.Publishers.Domain.Events;

namespace BooksRentalSystem.Publishers.Domain;

public class BookAdAggregate : Aggregate
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public decimal PricePerDay { get; set; }
    public int? PagesNumber { get; set; }
    public string Language { get; set; }
    public DateTime? PublicationDate { get; set; }
    public CoverType? CoverType { get; set; }
    public string Location { get; set; }
    public bool IsAvailable { get; set; }
    public int PublisherId { get; set; }
    public string AuthorName { get; set; }
    public int CategoryId { get; set; }
    public bool IsDeleted { get; set; }

    public void CreateBookAd(
        Guid id,
        string title,
        string description,
        string imageUrl,
        decimal pricePerDay,
        int? pagesNumber,
        string language,
        DateTime? publicationDate,
        CoverType? coverType,
        int publisherId,
        string authorName,
        int categoryId
    )
    {
        Apply(new BookAdCreatedEvent
        {
            Id = id,
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            PricePerDay = pricePerDay,
            PagesNumber = pagesNumber,
            Language = language,
            PublicationDate = publicationDate,
            CoverType = coverType,
            PublisherId = publisherId,
            AuthorName = authorName,
            CategoryId = categoryId
        });
    }

    public void UpdateBookAd(
        Guid id,
        string title,
        string description,
        string imageUrl,
        decimal pricePerDay,
        int? pagesNumber,
        string language,
        DateTime? publicationDate,
        CoverType? coverType,
        int publisherId,
        string authorName,
        int categoryId
    )
    {
        Apply(new BookAdUpdatedEvent
        {
            Id = id,
            Title = title,
            Description = description,
            ImageUrl = imageUrl,
            PricePerDay = pricePerDay,
            PagesNumber = pagesNumber,
            Language = language,
            PublicationDate = publicationDate,
            CoverType = coverType,
            PublisherId = publisherId,
            AuthorName = authorName,
            CategoryId = categoryId
        });
    }

    public void ChangeAvailability(Guid id, bool available)
    {
        Apply(new BookAdAvailabilityChangedEvent
        {
            Id = id,
            IsAvailable = available
        });
    }

    public void DeleteBookAd(Guid id)
    {
        Apply(new BookAdDeletedEvent
        {
            Id = id
        });
    }

    protected override void When(IEvent @event)
    {
        switch (@event)
        {
            case BookAdCreatedEvent e:
                OnBookAdCreated(e);
                break;
            case BookAdUpdatedEvent e:
                OnBookAdUpdated(e);
                break;
            case BookAdAvailabilityChangedEvent e:
                OnBookAdAvailabilityChanged(e);
                break;
            case BookAdDeletedEvent e:
                OnBookAdDeleted(e);
                break;
        }
    }

    private void OnBookAdCreated(BookAdCreatedEvent e)
    {
        Title = e.Title;
        Description = e.Description;
        ImageUrl = e.ImageUrl;
        PricePerDay = e.PricePerDay;
        PagesNumber = e.PagesNumber;
        Language = e.Language;
        PublicationDate = e.PublicationDate;
        CoverType = e.CoverType;
        IsAvailable = false;
        PublisherId = e.PublisherId;
        AuthorName = e.AuthorName;
        CategoryId = e.CategoryId;
        IsDeleted = false;
    }

    private void OnBookAdUpdated(BookAdUpdatedEvent e)
    {
        Title = e.Title;
        Description = e.Description;
        ImageUrl = e.ImageUrl;
        PricePerDay = e.PricePerDay;
        PagesNumber = e.PagesNumber;
        Language = e.Language;
        PublicationDate = e.PublicationDate;
        CoverType = e.CoverType;
        PublisherId = e.PublisherId;
        AuthorName = e.AuthorName;
        CategoryId = e.CategoryId;
    }

    private void OnBookAdAvailabilityChanged(BookAdAvailabilityChangedEvent e)
    {
        IsAvailable = e.IsAvailable;
    }

    private void OnBookAdDeleted(BookAdDeletedEvent e)
    {
        IsDeleted = true;
    }
}