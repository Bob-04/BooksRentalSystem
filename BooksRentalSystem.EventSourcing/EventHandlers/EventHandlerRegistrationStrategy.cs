using BooksRentalSystem.EventSourcing.Common;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BooksRentalSystem.EventSourcing.EventHandlers;

internal sealed class EventHandlerRegistrationStrategy : RegistrationStrategy
{
    public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
    {
        Type eventHandlerGenericType = typeof(IEventHandler<,>);
        Type eventHandlerType = TryGetInheritedGenericInterface(descriptor.ServiceType, eventHandlerGenericType);
        if (!string.IsNullOrEmpty(eventHandlerType?.GenericTypeArguments[0].FullName))
            EventHandlerDescriptorBuilder.Store.AddServiceDependency(
                eventHandlerType.GenericTypeArguments[0].FullName,
                new EventHandlerDescriptor { ImplementationType = descriptor.ImplementationType });
    }

    private static Type TryGetInheritedGenericInterface(Type type, Type inheritedFrom)
    {
        if (!inheritedFrom.IsInterface)
            return null;
        while (true)
        {
            if (type == null) return null;
            foreach (Type implementedInterface in type.GetInterfaces().Where(i => i.IsGenericType))
            {
                if (implementedInterface.GetGenericTypeDefinition() == inheritedFrom)
                    return implementedInterface;
            }

            type = type.BaseType;
        }
    }
}