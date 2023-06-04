using BooksRentalSystem.Console.Domain.Enums;
using BooksRentalSystem.Console.Domain.Events;
using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;

namespace BooksRentalSystem.Console.Domain.Aggregates;

public class CalculatorAggregate : Aggregate
{
    public double CurrentNumber { get; set; }
    public Operation? LastOperation { get; set; }
    public string LastOperationMadeBy { get; set; }

    public void MakeOperation(Operation operation, double number, string madeBy)
    {
        Apply(new OperationMadeEvent
        {
            Operation = operation,
            Number = number,
            MadeBy = madeBy
        });
    }

    public void Clear(string madeBy)
    {
        Apply(new ClearNumberEvent
        {
            MadeBy = madeBy
        });
    }

    protected override void When(IEvent @event)
    {
        switch (@event)
        {
            case OperationMadeEvent e:
                switch (e.Operation)
                {
                    case Operation.Add:
                        CurrentNumber += e.Number;
                        break;
                    case Operation.Subtract:
                        CurrentNumber -= e.Number;
                        break;
                    case Operation.Multiply:
                        CurrentNumber *= e.Number;
                        break;
                    case Operation.Divide:
                        CurrentNumber /= e.Number;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                LastOperation = e.Operation;
                LastOperationMadeBy = e.MadeBy;
                break;
            case ClearNumberEvent e:
                CurrentNumber = 0.0;
                LastOperation = null;
                LastOperationMadeBy = e.MadeBy;
                break;
        }
    }
}