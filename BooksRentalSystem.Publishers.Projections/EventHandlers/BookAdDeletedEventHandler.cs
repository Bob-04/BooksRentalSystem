using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Publishers.Domain.Events;
using BooksRentalSystem.Publishers.Projections.Services;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class BookAdDeletedEventHandler : EventStoreEventHandler<BookAdDeletedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public BookAdDeletedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        BookAdDeletedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var bookAdsService = serviceScope.ServiceProvider.GetRequiredService<IBookAdsService>();

        var isDeleted = await bookAdsService.Delete(@event.Id);
        if (isDeleted)
            await bookAdsService.Save();

        return default;
    }

    public override async Task AfterActionApplierAsync(
        BookAdDeletedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}