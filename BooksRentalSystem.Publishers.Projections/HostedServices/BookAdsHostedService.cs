using BooksRentalSystem.EventSourcing;
using BooksRentalSystem.EventSourcing.HostedServices;
using BooksRentalSystem.Publishers.Domain;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.HostedServices;

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
    protected override string GroupName => Constants.Subscriptions.BookAds.PublishersBookAdsGroupName;
    protected override string ClusterType => "Infra";
}