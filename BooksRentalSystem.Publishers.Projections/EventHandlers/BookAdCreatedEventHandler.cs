using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Domain.Events;
using BooksRentalSystem.Publishers.Projections.Services;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class BookAdCreatedEventHandler : EventStoreEventHandler<BookAdCreatedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public BookAdCreatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        BookAdCreatedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var categoryService = serviceScope.ServiceProvider.GetRequiredService<ICategoryService>();
        var authorsService = serviceScope.ServiceProvider.GetRequiredService<IAuthorsService>();
        var bookAdsService = serviceScope.ServiceProvider.GetRequiredService<IBookAdsService>();

        var category = await categoryService.Find(@event.CategoryId);
        if (category == default)
            throw new Exception("Category does not exist.");

        var author = await authorsService.FindByName(@event.AuthorName);
        author ??= new Author
        {
            Name = @event.AuthorName
        };

        var bookAdExists = await bookAdsService.Exists(@event.Id);
        if (!bookAdExists)
        {
            var bookAd = new BookAd
            {
                AggregateId = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                ImageUrl = @event.ImageUrl,
                PricePerDay = @event.PricePerDay,
                PublisherId = @event.PublisherId,
                Author = author,
                Category = category,
                BookInfo = new BookInfo
                {
                    PagesNumber = @event.PagesNumber,
                    Language = @event.Language,
                    PublicationDate = @event.PublicationDate,
                    CoverType = (CoverType?)@event.CoverType
                }
            };

            bookAdsService.Add(bookAd);

            // var message = new BookAdCreatedMessage
            // {
            //     BookAdId = bookAd.Id,
            //     Title = bookAd.Title,
            //     Author = bookAd.Author.Name,
            //     PricePerDay = bookAd.PricePerDay
            // };

            await bookAdsService.Save();
        }

        return default;
    }

    public override async Task AfterActionApplierAsync(
        BookAdCreatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}