using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Publishers.Domain.Events;
using BooksRentalSystem.Publishers.Projections.Services;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class BookAdAvailabilityChangedEventHandler : EventStoreEventHandler<BookAdAvailabilityChangedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public BookAdAvailabilityChangedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        BookAdAvailabilityChangedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var bookAdsService = serviceScope.ServiceProvider.GetRequiredService<IBookAdsService>();

        var bookAd = await bookAdsService.Find(@event.Id);
        if (bookAd == default)
            throw new NullReferenceException($"Book Ad with id {@event.Id} not found");

        bookAd.IsAvailable = @event.IsAvailable;
        await bookAdsService.Save();

        return default;
    }

    public override async Task AfterActionApplierAsync(
        BookAdAvailabilityChangedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}