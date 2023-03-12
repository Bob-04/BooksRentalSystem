using BooksRentalSystem.Console.Domain.Events;
using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Console.Domain.Aggregates;

public class TestCounterAggregate : Aggregate
{
    public int Counter { get; set; }

    public void IncreaseCounter()
    {
        Apply(new IncreaseTestCounterEvent());
    }

    public void DecreaseCounter()
    {
        Apply(new DecreaseTestCounterEvent());
    }

    protected override void When(IEvent @event)
    {
        switch (@event)
        {
            case IncreaseTestCounterEvent e:
                Counter++;
                break;
            case DecreaseTestCounterEvent e:
                Counter--;
                break;
        }
    }
}