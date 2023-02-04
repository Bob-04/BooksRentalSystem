using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Identity.Domain.Events;
using BooksRentalSystem.Publishers.Projections.Services;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class UserUpdatedEventHandler : EventStoreEventHandler<UserUpdatedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public UserUpdatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        UserUpdatedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var publishersService = serviceScope.ServiceProvider.GetRequiredService<IPublishersService>();

        var publisher = await publishersService.FindByUser(@event.Id.ToString());
        if (publisher != default)
        {
            publisher.Name = @event.Name;
            publisher.PhoneNumber = @event.PhoneNumber;
            await publishersService.Save();
        }

        return default;
    }

    public override async Task AfterActionApplierAsync(
        UserUpdatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}