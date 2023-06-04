using BooksRentalSystem.EventSourcing.Aggregates;
using BooksRentalSystem.EventSourcing.Events;
using BooksRentalSystem.Identity.Domain.Events;

namespace BooksRentalSystem.Identity.Domain;

public class UserAggregate : Aggregate
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }

    public void CreateUser(Guid id, string name, string email, string phoneNumber)
    {
        Apply(new UserCreatedEvent
        {
            Id = id,
            Name = name,
            Email = email,
            PhoneNumber = phoneNumber
        });
    }

    public void UpdateUser(Guid id, string name, string phoneNumber)
    {
        Apply(new UserUpdatedEvent
        {
            Id = id,
            Name = name,
            PhoneNumber = phoneNumber
        });
    }

    protected override void When(IEvent @event)
    {
        switch (@event)
        {
            case UserCreatedEvent e:
                OnUserCreated(e);
                break;
            case UserUpdatedEvent e:
                OnUserUpdated(e);
                break;
        }
    }

    private void OnUserCreated(UserCreatedEvent e)
    {
        Name = e.Name;
        Email = e.Email;
        PhoneNumber = e.PhoneNumber;
    }

    private void OnUserUpdated(UserUpdatedEvent e)
    {
        Name = e.Name;
        PhoneNumber = e.PhoneNumber;
    }
}