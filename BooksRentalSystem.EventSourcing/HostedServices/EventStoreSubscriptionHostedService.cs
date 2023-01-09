using System.Diagnostics.CodeAnalysis;
using System.Text;
using BooksRentalSystem.EventSourcing.Common;
using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.EventSourcing.Halpers;
using BooksRentalSystem.EventSourcing.Serialization;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BooksRentalSystem.EventSourcing.HostedServices;

public abstract class EventStoreSubscriptionHostedService : IHostedService
{
    private const long EventsMaxCount = 4096;
    private const int NextTryCreatingDelay = 100;
    private const int ConnectionCount = 1; //
    private const int RetryMessageTimeout = 1; //
    private const bool DisableParkedMessageSubscription = false; //
    private const int LockRetryCountParked = 5;
    private const short SubscriptionBufferSize = 20;
    private const short ParkedSubscriptionBufferSize = 1;

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventStoreSubscriptionHostedService> _logger;
    private readonly string _parkedStreamName;
    private readonly EventStorePersistentSubscriptionsClient _persistentSubscriptionsClient;
    private readonly string _streamName;
    private PersistentSubscription _parkedPersistentSubscription;
    private PersistentSubscription[] _persistentSubscriptions;

    protected EventStoreSubscriptionHostedService(
        EventStorePersistentSubscriptionsClient persistentSubscriptionsClient,
        IServiceProvider serviceProvider,
        ILogger<EventStoreSubscriptionHostedService> logger
    )
    {
        _persistentSubscriptionsClient = persistentSubscriptionsClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _streamName = NameHelper.GetPersistentSubscriptionStreamName(AggregateName);
        _parkedStreamName = NameHelper.GetPersistentSubscriptionParkedStreamName(AggregateName, GroupName);
    }

    protected abstract string AggregateName { get; }
    protected abstract string GroupName { get; }
    protected abstract string ClusterType { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await SubscribeAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            await SubscribeAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (PersistentSubscription persistentSubscription in _persistentSubscriptions)
            persistentSubscription?.Dispose();
        _parkedPersistentSubscription?.Dispose();

        return Task.CompletedTask;
    }

    private bool IsSubscriptionAvailable()
    {
        if (_persistentSubscriptions.All(s => s == default))
            return false;

        return DisableParkedMessageSubscription || _parkedPersistentSubscription != default;
    }

    private async Task SubscribeAsync(PersistentSubscription droppedPersistentSubscription = null)
    {
        if (droppedPersistentSubscription == default)
        {
            foreach (PersistentSubscription persistentSubscription in _persistentSubscriptions ??
                                                                      ArraySegment<PersistentSubscription>.Empty)
                persistentSubscription.Dispose();

            List<PersistentSubscription> subscriptions = new();
            List<Task<PersistentSubscription>> subscriptionTasks = Enumerable
                .Range(0, ConnectionCount)
                .Select(_ => _persistentSubscriptionsClient
                    .SubscribeToStreamAsync(_streamName, GroupName, EventAppeared, SubscriptionDropped,
                        bufferSize: SubscriptionBufferSize)).ToList();

            for (int i = 0; i < subscriptionTasks.Count; i++)
            {
                int retryCreateSubscriptionCount = 10;
                int iteration = 1;
                while (retryCreateSubscriptionCount > 0)
                    try
                    {
                        _logger.LogInformation($"Starting subscribe to {_streamName} {GroupName}.");
                        subscriptions.Add(await subscriptionTasks[i]);
                        _logger.LogInformation($"Success subscribe to {_streamName} {GroupName}." +
                                               $"Added {i + 1} of {subscriptionTasks.Count} connection.");
                        break;
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Fail subscribe to {_streamName} {GroupName}." +
                                                    $"Retry {iteration} of {iteration + retryCreateSubscriptionCount - 1}.");
                        await Task.Delay(TimeSpan.FromMilliseconds(NextTryCreatingDelay));
                        retryCreateSubscriptionCount--;
                        iteration++;
                    }
            }

            _persistentSubscriptions = subscriptions.ToArray();

            if (!DisableParkedMessageSubscription)
            {
                _parkedPersistentSubscription = await _persistentSubscriptionsClient
                    .SubscribeToStreamAsync(_parkedStreamName, GroupName, ParkedEventAppeared, SubscriptionDropped,
                        bufferSize: ParkedSubscriptionBufferSize);
            }
        }
        else
        {
            _logger.LogInformation("Dropped subscription appeared. {DroppedSubscription}",
                droppedPersistentSubscription.SubscriptionId);
            int index = Array.IndexOf(_persistentSubscriptions, droppedPersistentSubscription);
            if (index >= 0)
            {
                _persistentSubscriptions[index] = default;
                _persistentSubscriptions[index] = await _persistentSubscriptionsClient
                    .SubscribeToStreamAsync(_streamName, GroupName, EventAppeared, SubscriptionDropped,
                        bufferSize: SubscriptionBufferSize);
                _logger.LogInformation(
                    "Dropped (not parked)  {DroppedSubscription} subscription replaced by {AddedSubscription}, ",
                    droppedPersistentSubscription.SubscriptionId, _persistentSubscriptions[index].SubscriptionId
                );
            }

            if (!DisableParkedMessageSubscription)
            {
                if (_parkedPersistentSubscription == droppedPersistentSubscription)
                {
                    _parkedPersistentSubscription = default;
                    _parkedPersistentSubscription = await _persistentSubscriptionsClient
                        .SubscribeToStreamAsync(_parkedStreamName, GroupName, ParkedEventAppeared,
                            SubscriptionDropped, bufferSize: ParkedSubscriptionBufferSize);
                    _logger.LogInformation(
                        "Dropped parked {DroppedSubscription} subscription replaced by {AddedSubscription}, ",
                        droppedPersistentSubscription.SubscriptionId, _parkedPersistentSubscription.SubscriptionId
                    );
                }
            }
        }
    }

    private async Task HandleEventAsync(
        ResolvedEvent triggerEvent,
        IList<ResolvedEvent> events,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEventUpgradeDispatcher eventUpgradeDispatcher =
            _serviceProvider.GetRequiredService<IEventUpgradeDispatcher>();
        ServiceDependencyStore<EventHandlerDescriptor> eventHandlerDescriptorStore =
            _serviceProvider.GetRequiredService<ServiceDependencyStore<EventHandlerDescriptor>>();
        IEventsMappingProvider eventsMappingProvider =
            _serviceProvider.GetRequiredService<IEventsMappingProvider>();
        IEventStoreJsonSerializer jsonSerializer =
            _serviceProvider.GetRequiredService<IEventStoreJsonSerializer>();
        EventStoreClient eventStoreClient =
            _serviceProvider.GetRequiredService<EventStoreClient>();

        EventHandledResult eventHandledResult = new();
        try
        {
            eventHandledResult = await ProcessEventAsync(
                eventHandledResult,
                triggerEvent,
                _serviceProvider,
                eventUpgradeDispatcher,
                eventHandlerDescriptorStore,
                eventsMappingProvider,
                jsonSerializer,
                cancellationToken
            );

            if (eventHandledResult.DocumentToUpdate != default)
            {
                if (events == default)
                {
                    EventStoreClient.ReadStreamResult slice = eventStoreClient.ReadStreamAsync(
                        Direction.Forwards,
                        triggerEvent.Event.EventStreamId,
                        triggerEvent.Event.EventNumber.Next(),
                        EventsMaxCount,
                        resolveLinkTos: true,
                        cancellationToken: cancellationToken
                    );
                    events = await slice.ToListAsync(cancellationToken);
                }

                if (events != null && events.Any())
                {
                    foreach (ResolvedEvent resolvedEvent in events)
                    {
                        eventHandledResult = await ProcessEventAsync(
                            eventHandledResult,
                            resolvedEvent,
                            _serviceProvider,
                            eventUpgradeDispatcher,
                            eventHandlerDescriptorStore,
                            eventsMappingProvider,
                            jsonSerializer,
                            cancellationToken
                        );
                        if (eventHandledResult.DocumentToUpdate == default)
                            break;
                    }
                }
            }
        }
        finally
        {
            if (eventHandledResult.InitialEventHandler != null)
            {
                await eventHandledResult.InitialEventHandler.AfterActionApplierAsync(
                    eventHandledResult.InitialEvent,
                    eventHandledResult.DocumentToUpdate,
                    eventHandledResult.AfterSavingActions,
                    cancellationToken
                );
            }
        }
    }

    private static async Task<EventHandledResult> ProcessEventAsync(
        EventHandledResult eventHandledResult,
        ResolvedEvent triggerEvent,
        IServiceProvider serviceProvider,
        IEventUpgradeDispatcher eventUpgradeDispatcher,
        ServiceDependencyStore<EventHandlerDescriptor> eventHandlerDescriptorStore,
        IEventsMappingProvider eventsMappingProvider,
        IEventStoreJsonSerializer jsonSeserializer,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!TryGetEvent(triggerEvent, out IEvent @event, eventsMappingProvider, jsonSeserializer))
            throw new Exception("Cannot resolve event");

        IEvent upgradedEvent = eventUpgradeDispatcher.Dispatch(@event);

        EventHandlerDescriptor eventHandlerDescriptor =
            eventHandlerDescriptorStore.GetOptionalServiceDependency(@event.GetType().FullName);
        if (eventHandlerDescriptor == default)
        {
            if (eventHandledResult.SkipEventHandler == default)
            {
                EventHandlerDescriptor defaultStreamEventHandler =
                    eventHandlerDescriptorStore.GetOptionalServiceDependency(
                        NameHelper.GetStreamName(triggerEvent.Event.EventStreamId));

                if (defaultStreamEventHandler == default)
                    throw new Exception($"Event not handled, Event: {upgradedEvent}");
                if (serviceProvider.GetRequiredService(defaultStreamEventHandler.ImplementationType) is not
                    ISkipEventHandler skipEventHandler)
                    throw new Exception($"Event not handled, Event: {upgradedEvent}");

                eventHandledResult = eventHandledResult with { SkipEventHandler = skipEventHandler };
            }

            eventHandledResult.SkipEventHandler.Validate(@event);
            return eventHandledResult with { InitialEvent = eventHandledResult.InitialEvent ?? @event };
        }

        if (serviceProvider.GetRequiredService(eventHandlerDescriptor.ImplementationType) is not
            IEventHandler eventHandler) return eventHandledResult;

        (object documentToUpdate, AfterSavingActionStore afterSavingAction) =
            await eventHandler.HandleAsync(
                upgradedEvent,
                triggerEvent,
                eventHandledResult.DocumentToUpdate,
                eventHandledResult.AfterSavingActions,
                cancellationToken
            );

        return eventHandledResult with
        {
            DocumentToUpdate = documentToUpdate,
            AfterSavingActions = afterSavingAction,
            InitialEventHandler = eventHandledResult.InitialEventHandler ?? eventHandler,
            InitialEvent = eventHandledResult.InitialEvent ?? @event
        };
    }

    [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
    private async Task EventAppeared(
        PersistentSubscription persistentSubscription,
        ResolvedEvent triggerEvent,
        int? retryNumber,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (triggerEvent.Event == default)
        {
            await persistentSubscription.Ack(triggerEvent);
            return;
        }

        _logger.BeginScope($"{nameof(EventAppeared)}.{triggerEvent.Event.EventId}");
        bool doesEventCameFromParked = triggerEvent.OriginalStreamId.Contains("parked");
        try
        {
            if (triggerEvent.Event.EventStreamId.StartsWith("$$") || triggerEvent.Event.EventType == "$metadata")
            {
                LogSkip(triggerEvent, $"Event {triggerEvent.Event.EventId} is deleted.");
                await persistentSubscription.Ack(triggerEvent);
                return;
            }

            LogAccept(triggerEvent);
            await HandleEventAsync(
                triggerEvent,
                null,
                cancellationToken
            );
            LogAck(triggerEvent);
            await persistentSubscription.Ack(triggerEvent);
            _logger.LogTrace("Event {EventId} was resolved", triggerEvent.Event.EventId);
        }
        catch (Exception e)
        {
            LogPark(triggerEvent, e);
            await persistentSubscription.Nack(PersistentSubscriptionNakEventAction.Park, e.StackTrace ?? e.Message,
                triggerEvent);
        }
    }

    private async Task ParkedEventAppeared(
        PersistentSubscription persistentSubscription,
        ResolvedEvent triggerEvent,
        int? retryNumber,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (triggerEvent.Event == default)
            return;

        _logger.BeginScope($"{nameof(ParkedEventAppeared)}.{triggerEvent.Event.EventId}");

        await persistentSubscription.Ack(triggerEvent);

        if (triggerEvent.Event.EventStreamId.StartsWith("$$") || triggerEvent.Event.EventType == "$metadata")
            return;

        EventStoreClient eventStoreClient =
            _serviceProvider.GetRequiredService<EventStoreClient>();
        long nextPageStart = default;
        do
        {
            EventStoreClient.ReadStreamResult slice = eventStoreClient.ReadStreamAsync(
                Direction.Forwards,
                triggerEvent.Event.EventStreamId,
                StreamPosition.FromInt64(nextPageStart),
                EventsMaxCount,
                resolveLinkTos: true,
                cancellationToken: cancellationToken
            );

            IList<ResolvedEvent> events = await slice.ToListAsync(cancellationToken);

            if (events != null && events.Any())
            {
                ResolvedEvent initialEvent = events.First();

                short lockRetryCount = 0;
                short versionMismatchRetryCount = 0;
                short appliedVersionRetryCount = 0;
                do
                {
                    LogAccept(initialEvent, true);
                    try
                    {
                        await HandleEventAsync(
                            initialEvent,
                            events
                                .SkipWhile(resolvedEvent =>
                                    initialEvent.Event.EventNumber != resolvedEvent.Event.EventNumber)
                                .Skip(1)
                                .ToList(),
                            cancellationToken
                        );
                        await persistentSubscription.Ack(triggerEvent);
                        LogAck(initialEvent, true);

                        _logger.LogInformation("Event {EventId} was resolved from parked", triggerEvent.Event.EventId);

                        initialEvent = new ResolvedEvent();
                    }
                    catch (Exception ex)
                    {
                        _logger
                            .LogError("Unhandled exception was thrown during parked event {EventId} reprocessing",
                                initialEvent.Event.EventId);
                        LogPark(initialEvent, ex, true);
                        break;
                    }
                } while (initialEvent.Event != null);

                nextPageStart = events.Count == EventsMaxCount
                    ? events.Last().Event.EventNumber.Next().ToInt64()
                    : -1;
            }
            else
                nextPageStart = -1;
        } while (nextPageStart != -1);
    }

    private void SubscriptionDropped(
        PersistentSubscription persistentSubscription,
        SubscriptionDroppedReason reason,
        Exception exception
    )
    {
        _logger.LogError(exception, reason.ToString());

        if (reason == SubscriptionDroppedReason.Disposed)
            return;

        SubscribeAsync(persistentSubscription).GetAwaiter().GetResult();
    }

    private static bool TryGetEvent(
        ResolvedEvent resolvedEvent,
        out IEvent? @event,
        IEventsMappingProvider eventsMappingProvider,
        IEventStoreJsonSerializer jsonDeserializer
    )
    {
        var eventType = eventsMappingProvider.GetEventType(resolvedEvent.Event.EventType);
        if (eventType == null)
        {
            @event = null;
            return false;
        }

        var data = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
        @event = jsonDeserializer.Deserialize(data, eventType) as IEvent;
        @event!.SetEventCreatedAt(resolvedEvent.Event.Created);
        @event!.SetEventNumber(resolvedEvent.Event.EventNumber.ToInt64());

        return true;
    }

    private void LogAccept(ResolvedEvent e, bool isParked = false)
    {
        _logger.LogInformation(
            $"Update {AggregateName}{ParkedSuffix(isParked)} (Accept) - " +
            $"Stream Id: {e.Event.EventStreamId} | " +
            $"Event Id: {e.Event.EventId} | " +
            $"Event Stream Number: {e.Event.EventNumber} | " +
            $"Event Number: {e.OriginalEventNumber}"
        );
    }

    private void LogAck(ResolvedEvent e, bool isParked = false)
    {
        _logger.LogInformation(
            $"Update {AggregateName}{ParkedSuffix(isParked)} (Ack) - " +
            $"Stream Id: {e.Event.EventStreamId} | " +
            $"Event Id: {e.Event.EventId} | " +
            $"Event Stream Number: {e.Event.EventNumber} | " +
            $"Event Number: {e.OriginalEventNumber}"
        );
    }

    private void LogSkip(ResolvedEvent e, string reason, bool isParked = false)
    {
        _logger.LogError(
            $"Update {AggregateName}{ParkedSuffix(isParked)} (Skip) - " +
            $"Stream Id: {e.Event.EventStreamId} | " +
            $"Event Id: {e.Event.EventId} | " +
            $"Event Stream Number: {e.Event.EventNumber} | " +
            $"Event Number: {e.OriginalEventNumber} | " +
            $"Reason: {reason}"
        );
    }

    private void LogRetry(ResolvedEvent e, string reason, bool isParked = false)
    {
        _logger.LogWarning(
            $"Update {AggregateName}{ParkedSuffix(isParked)} (Retry) - " +
            $"Stream Id: {e.Event.EventStreamId} | " +
            $"Event Id: {e.Event.EventId} | " +
            $"Event Stream Number: {e.Event.EventNumber} | " +
            $"Event Number: {e.OriginalEventNumber} | " +
            $"Reason: {reason}"
        );
    }

    private void LogPark(ResolvedEvent e, Exception ex, bool isParked = false)
    {
        _logger.LogError(
            $"Update {AggregateName}{ParkedSuffix(isParked)} (Park) - " +
            $"Stream Id: {e.Event.EventStreamId} | " +
            $"Event Id: {e.Event.EventId} | " +
            $"Event Stream Number: {e.Event.EventNumber} | " +
            $"Event Number: {e.OriginalEventNumber} | " +
            $"Reason: {ex.Message} | " +
            $"Trace: {ex.StackTrace}"
        );
    }

    private string ParkedSuffix(bool isParked)
    {
        return isParked ? "-parked" : string.Empty;
    }
}