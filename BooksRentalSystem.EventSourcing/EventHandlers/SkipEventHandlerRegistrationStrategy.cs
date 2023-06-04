using BooksRentalSystem.EventSourcing.Common;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace BooksRentalSystem.EventSourcing.EventHandlers;

internal sealed class SkipEventHandlerRegistrationStrategy : RegistrationStrategy
{
    public override void Apply(IServiceCollection services, ServiceDescriptor descriptor)
    {
        if (descriptor?.ImplementationType?.BaseType?.GenericTypeArguments[0].Name != default)
            EventHandlerDescriptorBuilder.Store.AddServiceDependency(
                descriptor.ImplementationType.BaseType.GenericTypeArguments[0].Name,
                new EventHandlerDescriptor { ImplementationType = descriptor.ImplementationType });
    }
}