using BooksRentalSystem.EventSourcing.Events;
using Newtonsoft.Json;

namespace BooksRentalSystem.EventSourcing.Aggregates;

public abstract class Aggregate
{
    [JsonIgnore] private readonly IList<IEvent> _changes;
    public DateTime AggregateCreatedAt;
    public DateTime AggregateUpdatedAt;

    protected Aggregate()
    {
        _changes = new List<IEvent>();
    }

    public Guid Id { get; set; }
    public long Version { get; set; } = -1;

    [JsonIgnore] public virtual DateTime CreatedAt => AggregateCreatedAt;
    [JsonIgnore] public virtual DateTime UpdatedAt => AggregateUpdatedAt;

    protected virtual void PreApply(IEvent @event)
    {
        if (Version == -1 && AggregateCreatedAt == default)
            AggregateCreatedAt = @event.EventCreatedAt != default ? @event.EventCreatedAt : DateTime.UtcNow;

        AggregateUpdatedAt = @event.EventCreatedAt != default ? @event.EventCreatedAt : DateTime.UtcNow;
    }

    protected abstract void When(IEvent @event);

    protected void Apply(IEvent @event)
    {
        PreApply(@event);
        When(@event);

        _changes.Add(@event);
    }

    public void Load(IEnumerable<IEvent> history)
    {
        foreach (IEvent e in history)
        {
            PreApply(e);
            When(e);
            Version = e.EventNumber + 1;
        }
    }

    public IEnumerable<IEvent> GetChanges()
    {
        return _changes.ToArray();
    }
}