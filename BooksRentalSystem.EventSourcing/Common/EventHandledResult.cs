using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.EventSourcing.Common;

internal sealed record EventHandledResult
{
    public ISkipEventHandler SkipEventHandler { get; init; }
    public IEventHandler InitialEventHandler { get; init; }
    public IEvent InitialEvent { get; init; }
    public object DocumentToUpdate { get; init; }
    public AfterSavingActionStore AfterSavingActions { get; init; }
}