namespace BooksRentalSystem.EventSourcing.Common;

internal sealed class EventHandlerDescriptorBuilder
{
    public static readonly ServiceDependencyStore<EventHandlerDescriptor> Store = new();
}