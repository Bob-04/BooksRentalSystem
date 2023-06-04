namespace BooksRentalSystem.EventSourcing.Events;

public interface IEvent
{
    DateTime EventCreatedAt { get; protected set; }
    long EventNumber { get; protected set; }

    void SetEventCreatedAt(DateTime dateTime) => EventCreatedAt = dateTime;
    void SetEventNumber(long eventNumber) => EventNumber = eventNumber;
}