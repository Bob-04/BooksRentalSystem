using BooksRentalSystem.EventSourcing.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.EventSourcing.Events;

public interface IEventUpgraderFactory
{
    IEventUpgrader GetEventUpgrader(Type eventType);
}

public sealed class EventUpgraderFactory : IEventUpgraderFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventUpgraderFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public IEventUpgrader GetEventUpgrader(Type eventType)
    {
        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();

        ServiceDependencyStore<Type> eventUpgraderResolver = serviceScope
            .ServiceProvider
            .GetService<ServiceDependencyStore<Type>>();
        Type eventUpgraderType = eventUpgraderResolver?.GetOptionalServiceDependency(eventType.FullName);
        if (eventUpgraderType != null)
        {
            IEventUpgrader eventUpgrader = serviceScope
                .ServiceProvider
                .GetRequiredService(eventUpgraderType) as IEventUpgrader;

            return eventUpgrader;
        }

        return null;
    }
}