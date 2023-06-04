using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.EventSourcing.EventHandlers;

internal interface ISkipEventHandler
{
    void Validate(IEvent @event);
}

public abstract class EventStoreSkipEventHandler<TAggregate> : ISkipEventHandler where TAggregate : Aggregate
{
    protected abstract IEnumerable<Type> SkippedEventTypes { get; }

    public void Validate(IEvent @event)
    {
        if (SkippedEventTypes.Any(et => et == @event.GetType()))
            return;
        throw new Exception($"Event not handled, Event: {@event}");
    }
}