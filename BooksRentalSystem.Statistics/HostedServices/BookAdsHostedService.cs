﻿using System;
using BooksRentalSystem.EventSourcing;
using BooksRentalSystem.EventSourcing.HostedServices;
using BooksRentalSystem.Publishers.Domain;
using EventStore.Client;
using Microsoft.Extensions.Logging;

namespace BooksRentalSystem.Statistics.HostedServices;

public class BookAdsHostedService : EventStoreSubscriptionHostedService
{
    public BookAdsHostedService(
        EventStorePersistentSubscriptionsClient persistentSubscriptionsClient,
        IServiceProvider serviceProvider,
        ILogger<EventStoreSubscriptionHostedService> logger
    ) : base(persistentSubscriptionsClient, serviceProvider, logger)
    {
    }

    protected override string AggregateName => nameof(BookAdAggregate);
    protected override string GroupName => Constants.Subscriptions.BookAds.StatisticsBookAdsGroupName;
    protected override string ClusterType => "Infra";
}