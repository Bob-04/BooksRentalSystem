using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.EventSourcing.Halpers;
using BooksRentalSystem.EventSourcing.Serialization;
using BooksRentalSystem.EventSourcing.Snapshotting;
using EventStore.Client;

namespace BooksRentalSystem.EventSourcing.Repositories;

public interface IEventStoreAggregateRepository
{
    Task SaveAsync<TAggregate>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new();

    Task<TAggregate> LoadAsync<TAggregate>(
        Guid aggregateId,
        TAggregate? baseAggregate = default,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new();
}

public class EventStoreAggregateRepository : IEventStoreAggregateRepository
{
    private const long TakeSnapshotAfterEventsCount = 100;

    private readonly EventStoreClient _eventStoreClient;
    private readonly ISnapshotStore _snapshotStore;
    private readonly IEventStoreJsonSerializer _jsonSerializer;
    private readonly bool _allowSnapshotting;
    private static IDictionary<string, Type> _eventsTypeMap = new Dictionary<string, Type>();

    public EventStoreAggregateRepository(EventStoreClient eventStoreClient, ISnapshotStore snapshotStore,
        IEventStoreJsonSerializer jsonSerializer, bool allowSnapshotting = true)
    {
        _eventStoreClient = eventStoreClient;
        _snapshotStore = snapshotStore;
        _jsonSerializer = jsonSerializer;
        _allowSnapshotting = allowSnapshotting;

        _eventsTypeMap = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "BooksRentalSystem.*.Domain.dll")
            .Select(a => Assembly.Load(AssemblyName.GetAssemblyName(a)))
            .SelectMany(a => a.DefinedTypes
                .Where(t => Regex.IsMatch(t.Name, @"\w*Event\b")))
            .Select(t => t.AsType())
            .ToDictionary(t => t.Name.ToLower());
    }

    public async Task SaveAsync<TAggregate>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        var events = aggregate.GetChanges()
            .Select(e => new EventData(
                    Uuid.NewUuid(),
                    e.GetType().Name,
                    Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(e)),
                    Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(CreateEventMetadata(e)))
                )
            ).ToArray();

        if (!events.Any())
            return;

        var aggregateKey = NameHelper.GetStreamName(aggregate, aggregate.Id);

        var writeResult = await _eventStoreClient.AppendToStreamAsync(
            aggregateKey,
            StreamState.Any,
            events,
            cancellationToken: cancellationToken
        );

        var version = aggregate.Version == -1 ? 0 : aggregate.Version;
        var lastEventVersion = writeResult.NextExpectedStreamRevision.Next().ToInt64();
        var nextSnapshotVersion = lastEventVersion - lastEventVersion % TakeSnapshotAfterEventsCount;

        if (_allowSnapshotting && version < nextSnapshotVersion && nextSnapshotVersion <= lastEventVersion)
        {
            var oldSnapshot = await _snapshotStore.GetByVersionOrLast<TAggregate>(aggregate.Id.ToString());
            if (oldSnapshot == default)
            {
                var aggregateForSnapshot = await LoadAggregateAsync(
                    new TAggregate { Id = aggregate.Id },
                    Direction.Forwards,
                    aggregateKey,
                    StreamPosition.Start,
                    nextSnapshotVersion,
                    cancellationToken: cancellationToken
                );

                await _snapshotStore.Save(aggregateForSnapshot);
            }
            else
            {
                var aggregateForSnapshot = await LoadAggregateAsync(
                    oldSnapshot,
                    Direction.Forwards,
                    aggregateKey,
                    StreamPosition.FromInt64(oldSnapshot.Version),
                    nextSnapshotVersion - oldSnapshot.Version,
                    cancellationToken: cancellationToken
                );

                await _snapshotStore.Save(aggregateForSnapshot);
            }
        }
    }

    public async Task<TAggregate> LoadAsync<TAggregate>(
        Guid aggregateId,
        TAggregate? baseAggregate = default,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        var aggregate = baseAggregate ?? new TAggregate { Id = aggregateId };

        var aggregateKey = NameHelper.GetStreamName(aggregate, aggregateId);

        if (!_allowSnapshotting)
            return await LoadAggregateAsync(
                aggregate,
                Direction.Forwards,
                aggregateKey,
                StreamPosition.Start,
                cancellationToken: cancellationToken
            );

        var latestSlice = _eventStoreClient.ReadStreamAsync(
            Direction.Backwards,
            aggregateKey,
            StreamPosition.End,
            TakeSnapshotAfterEventsCount,
            resolveLinkTos: true,
            cancellationToken: cancellationToken
        );

        var latestEvents = await latestSlice.ToListAsync(cancellationToken);

        if (!latestEvents.Any())
            return aggregate;

        if (latestEvents.Count == TakeSnapshotAfterEventsCount)
        {
            var snapshot = await _snapshotStore.GetByVersionOrLast<TAggregate>(aggregateId.ToString());

            if (snapshot == default)
                return await LoadAggregateAsync(
                    aggregate,
                    Direction.Forwards,
                    aggregateKey,
                    StreamPosition.Start,
                    cancellationToken: cancellationToken
                );

            aggregate = snapshot;

            var latestEventNumber = latestEvents.First().Event.EventNumber.ToInt64() + 1;
            var eventsCountAfterSnapshot = latestEventNumber % TakeSnapshotAfterEventsCount;

            if (eventsCountAfterSnapshot == 0)
            {
                if (aggregate.Version == latestEventNumber)
                    return aggregate;

                return await LoadAggregateAsync(
                    aggregate,
                    Direction.Forwards,
                    aggregateKey,
                    StreamPosition.FromInt64(aggregate.Version),
                    cancellationToken: cancellationToken
                );
            }

            var unAppliedEvents = latestEvents
                .Take((int)eventsCountAfterSnapshot)
                .Reverse()
                .ToList();

            if (aggregate.Version + 1 == unAppliedEvents.First().Event.EventNumber.ToInt64() + 1)
                return ApplyEventsToAggregate(aggregate, unAppliedEvents);

            return await LoadAggregateAsync(
                aggregate,
                Direction.Forwards,
                aggregateKey,
                StreamPosition.FromInt64(aggregate.Version),
                cancellationToken: cancellationToken
            );
        }

        return ApplyEventsToAggregate(aggregate, latestEvents.AsEnumerable().Reverse());
    }

    private static EventMetadata CreateEventMetadata(IEvent e)
    {
        return new EventMetadata
        {
            EventFullName = e.GetType().FullName
        };
    }

    private async Task<TAggregate> LoadAggregateAsync<TAggregate>(
        TAggregate aggregate,
        Direction direction,
        string streamName,
        StreamPosition revision,
        long takeCount = 9223372036854775807,
        bool resolveLinkTos = false,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        var slice = _eventStoreClient.ReadStreamAsync(
            direction,
            streamName,
            revision,
            takeCount,
            resolveLinkTos,
            cancellationToken: cancellationToken
        );

        var events = await slice.ToListAsync(cancellationToken);

        if (events.Any())
        {
            var latestEventNumber = events.First().Event.EventNumber.ToInt64() + 1;
            var eventsCountAfterSnapshot = latestEventNumber % TakeSnapshotAfterEventsCount;

            if (eventsCountAfterSnapshot == 0)
            {
                if (aggregate.Version == latestEventNumber)
                    return aggregate;
            }

            aggregate = ApplyEventsToAggregate(aggregate, events.AsEnumerable());
        }

        return aggregate;
    }

    private TAggregate ApplyEventsToAggregate<TAggregate>(
        TAggregate aggregate,
        IEnumerable<ResolvedEvent> resolvedEvents
    ) where TAggregate : Aggregate, new()
    {
        var history = Enumerable.Empty<IEvent>().ToList();
        foreach (var resolvedEvent in resolvedEvents)
        {
            _eventsTypeMap.TryGetValue(resolvedEvent.Event.EventType.ToLower(), out var resolvedEventType);

            if (resolvedEventType != default)
            {
                var resolvedEventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());

                if (_jsonSerializer.Deserialize(resolvedEventData, resolvedEventType) is IEvent @event)
                {
                    @event.SetEventCreatedAt(resolvedEvent.Event.Created);
                    @event.SetEventNumber(resolvedEvent.Event.EventNumber.ToInt64());
                    history.Add(@event);
                }
            }
        }

        aggregate.Load(history);

        return aggregate;
    }
}