using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Domain.Events;
using BooksRentalSystem.Publishers.Projections.Services;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class BookAdUpdatedEventHandler : EventStoreEventHandler<BookAdUpdatedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public BookAdUpdatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        BookAdUpdatedEvent @event,
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

        var bookAd = await bookAdsService.Find(@event.Id);
        if (bookAd == default)
            throw new NullReferenceException($"Book Ad with id {@event.Id} not found");

        bookAd.Title = @event.Title;
        bookAd.Description = @event.Description;
        bookAd.ImageUrl = @event.ImageUrl;
        bookAd.PricePerDay = @event.PricePerDay;
        bookAd.PublisherId = @event.PublisherId;
        bookAd.Author = author;
        bookAd.Category = category;
        bookAd.BookInfo = new BookInfo
        {
            PagesNumber = @event.PagesNumber,
            Language = @event.Language,
            PublicationDate = @event.PublicationDate,
            CoverType = (CoverType?)@event.CoverType
        };

        await bookAdsService.Save();

        return default;
    }

    public override async Task AfterActionApplierAsync(
        BookAdUpdatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}