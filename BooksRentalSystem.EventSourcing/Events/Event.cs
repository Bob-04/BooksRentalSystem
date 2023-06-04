using Newtonsoft.Json;

namespace BooksRentalSystem.EventSourcing.Events;

public record Event : IEvent
{
    [JsonIgnore]
    public DateTime EventCreatedAt => ((IEvent)this).EventCreatedAt != default
        ? ((IEvent)this).EventCreatedAt
        : DateTime.UtcNow;

    DateTime IEvent.EventCreatedAt { get; set; }

    long IEvent.EventNumber { get; set; }
}