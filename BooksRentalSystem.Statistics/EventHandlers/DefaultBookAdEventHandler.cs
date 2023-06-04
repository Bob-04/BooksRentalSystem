using System;
using System.Collections.Generic;
using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Publishers.Domain;
using BooksRentalSystem.Publishers.Domain.Events;

namespace BooksRentalSystem.Statistics.EventHandlers;

public class DefaultBookAdEventHandler : EventStoreSkipEventHandler<BookAdAggregate>
{
    protected override IEnumerable<Type> SkippedEventTypes  => new[]
    {
        typeof(BookAdUpdatedEvent),
        typeof(BookAdAvailabilityChangedEvent),
        typeof(BookAdDeletedEvent)
    };
}