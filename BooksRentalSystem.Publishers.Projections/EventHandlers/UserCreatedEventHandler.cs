using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Identity.Domain.Events;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class UserCreatedEventHandler : EventStoreEventHandler<UserCreatedEvent, object>
{
    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        UserCreatedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        Console.Write("yehoo!");

        await Task.CompletedTask;

        return default;
    }

    public override async Task AfterActionApplierAsync(
        UserCreatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}