namespace BooksRentalSystem.EventSourcing.EventHandlers;

public sealed class AfterSavingActionStore : Dictionary<string, Func<Task>>
{
    public AfterSavingActionStore()
    {
    }

    public AfterSavingActionStore(string name, Func<Task> afterSavingAction)
    {
        this[name] = afterSavingAction;
    }
}