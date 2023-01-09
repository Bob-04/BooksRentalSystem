using BooksRentalSystem.EventSourcing.Common;
using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.EventSourcing.HostedServices;
using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.EventSourcing.Serialization;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BooksRentalSystem.EventSourcing.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSourcing(this IServiceCollection services,
        string eventStoreConnectionString)
    {
        services.AddEventStoreClient(eventStoreConnectionString);

        services.AddSingleton<IEventStoreJsonSerializer, EventStoreJsonSerializer>();

        services.AddTransient<IEventStoreAggregateRepository, EventStoreAggregateRepository>();

        return services;
    }

    public static IServiceCollection AddEventStoreSubscription<TSubscriptionService>(this IServiceCollection services,
        string eventStoreConnectionString
    ) where TSubscriptionService : class, IHostedService
    {
        services.AddSingleton<IEventStoreJsonSerializer, EventStoreJsonSerializer>();

        services.AddSingleton<IEventUpgraderFactory, EventUpgraderFactory>();
        services.AddSingleton<IEventUpgradeDispatcher, EventUpgradeDispatcher>();

        services.AddSingleton<IEventsMappingProvider, EventsMappingProvider>();

        // services.AddSingleton<ServiceResolver<EventStoreClient>>(sp =>
        //     key => sp.GetRequiredService<ServiceDependencyStore<EventStoreClient>>()
        //         .GetRequiredServiceDependency(key)
        // );

        services.AddEventStoreClient(eventStoreConnectionString);
        services.AddEventStorePersistentSubscriptionsClient(eventStoreConnectionString);

        services
            .Scan(scan => scan
                .FromEntryAssembly()
                .AddClasses(itp => itp.AssignableTo(typeof(IEventHandler<,>)))
                .UsingRegistrationStrategy(new EventHandlerRegistrationStrategy())
            )
            .Scan(scan => scan
                .FromEntryAssembly()
                .AddClasses(itp => itp.AssignableTo(typeof(ISkipEventHandler)))
                .UsingRegistrationStrategy(new SkipEventHandlerRegistrationStrategy())
            )
            .AddHostedService<TSubscriptionService>();

        services.AddSingleton(EventHandlerDescriptorBuilder.Store);
        foreach (EventHandlerDescriptor descriptor in EventHandlerDescriptorBuilder.Store.GetServiceDependencies())
            services.AddTransient(descriptor.ImplementationType);

        return services;
    }

    private static Type TryGetInheritedGeneric(Type type, Type inheritedFrom)
    {
        while (true)
        {
            if (type.BaseType == null) return null;
            if (type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == inheritedFrom)
                return type.BaseType;
            type = type.BaseType;
        }
    }
}