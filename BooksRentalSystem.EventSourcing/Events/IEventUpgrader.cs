namespace BooksRentalSystem.EventSourcing.Events;

public interface IEventUpgrader
{
    IEvent UpgradeEvent(IEvent @event);
}