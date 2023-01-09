namespace BooksRentalSystem.EventSourcing.Events;

public interface IEventUpgradeDispatcher
{
    IEvent Dispatch(IEvent @event);
}

public sealed class EventUpgradeDispatcher : IEventUpgradeDispatcher
{
    private readonly IEventUpgraderFactory _eventUpgraderFactory;

    public EventUpgradeDispatcher(IEventUpgraderFactory eventUpgraderFactory)
    {
        _eventUpgraderFactory = eventUpgraderFactory;
    }

    public IEvent Dispatch(IEvent @event)
    {
        IEvent upgradedEvent = @event;
        IEventUpgrader eventUpgrader;
        do
        {
            eventUpgrader = _eventUpgraderFactory.GetEventUpgrader(upgradedEvent.GetType());
            if (eventUpgrader != null)
                upgradedEvent = eventUpgrader.UpgradeEvent(upgradedEvent);
        } while (eventUpgrader != null);

        return upgradedEvent;
    }
}