using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.EventSourcing.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.EventSourcing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSourcing(this IServiceCollection serviceCollection,
        string eventStoreConnectionString)
    {
        serviceCollection.AddEventStoreClient(eventStoreConnectionString);

        serviceCollection.AddSingleton<IEventStoreJsonSerializer, EventStoreJsonSerializer>();

        serviceCollection.AddTransient<IEventStoreAggregateRepository, EventStoreAggregateRepository>();

        return serviceCollection;
    }
}