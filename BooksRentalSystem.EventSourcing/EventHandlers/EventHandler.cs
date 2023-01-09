using BooksRentalSystem.EventSourcing.Events;
using EventStore.Client;

namespace BooksRentalSystem.EventSourcing.EventHandlers;

public interface IEventHandler
{
    Task<(object, AfterSavingActionStore)> HandleAsync(
        IEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    );

    Task AfterActionApplierAsync(
        IEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    );
}

public interface IEventHandler<in TEvent, TDocument> : IEventHandler
    where TEvent : class, IEvent
    where TDocument : class
{
    Task<(TDocument, AfterSavingActionStore)> HandleAsync(
        TEvent @event,
        ResolvedEvent resolvedEvent,
        TDocument documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    );

    Task AfterActionApplierAsync(
        TEvent @event,
        TDocument documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    );
}

public abstract class EventStoreEventHandler<TEvent, TDocument> : IEventHandler<TEvent, TDocument>
    where TEvent : class, IEvent
    where TDocument : class
{
    public abstract Task<(TDocument, AfterSavingActionStore)> HandleAsync(
        TEvent @event,
        ResolvedEvent resolvedEvent,
        TDocument documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    );

    async Task<(object, AfterSavingActionStore)> IEventHandler.HandleAsync(
        IEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken
    )
    {
        return await HandleAsync(
            TryConvertEvent(@event),
            resolvedEvent,
            documentToUpdate as TDocument,
            afterSaveActions ?? new AfterSavingActionStore(),
            cancellationToken
        );
    }

    async Task IEventHandler.AfterActionApplierAsync(
        IEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await AfterActionApplierAsync(TryConvertEvent(@event), documentToUpdate as TDocument, afterSaveActions,
            cancellationToken);

        if (afterSaveActions != default)
            foreach (var afterSaveAction in afterSaveActions.Values)
                await afterSaveAction();
    }

    public abstract Task AfterActionApplierAsync(
        TEvent @event,
        TDocument documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    );

    private static TEvent TryConvertEvent(IEvent @event)
    {
        if (@event is TEvent eventClass)
            return eventClass;

        throw new Exception(
            $"Expected event {typeof(TEvent).Name}, but received {@event.GetType().Name} with EventNumber {@event.EventNumber}, {@event.EventCreatedAt}");
    }
}