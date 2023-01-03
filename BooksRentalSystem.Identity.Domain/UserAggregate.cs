using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.Identity.Domain.Events;

namespace BooksRentalSystem.Identity.Domain;

public class UserAggregate : Aggregate
{
    public string Email { get; set; }

    public void CreateUser(string email)
    {
        Apply(new UserCreatedEvent
        {
            Email = email
        });
    }

    protected override void When(IEvent @event)
    {
        switch (@event)
        {
            case UserCreatedEvent e:
                OnUserCreated(e);
                break;
        }
    }

    private void OnUserCreated(UserCreatedEvent e)
    {
        Email = e.Email;
    }
}