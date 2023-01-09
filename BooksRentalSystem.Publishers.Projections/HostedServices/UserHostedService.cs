using BooksRentalSystem.EventSourcing;
using BooksRentalSystem.EventSourcing.HostedServices;
using BooksRentalSystem.Identity.Domain;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.HostedServices;

public class UserHostedService : EventStoreSubscriptionHostedService
{
    public UserHostedService(
        EventStorePersistentSubscriptionsClient persistentSubscriptionsClient,
        IServiceProvider serviceProvider,
        ILogger<EventStoreSubscriptionHostedService> logger
    ) : base(persistentSubscriptionsClient, serviceProvider, logger)
    {
    }

    protected override string AggregateName => nameof(UserAggregate);
    protected override string GroupName => Constants.Subscriptions.UsersGroupName;
    protected override string ClusterType => "Infra";
}