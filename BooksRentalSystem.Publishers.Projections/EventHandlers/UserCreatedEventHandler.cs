using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Identity.Domain.Events;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Projections.Services;
using EventStore.Client;

namespace BooksRentalSystem.Publishers.Projections.EventHandlers;

public class UserCreatedEventHandler : EventStoreEventHandler<UserCreatedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public UserCreatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        UserCreatedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var publishersService = serviceScope.ServiceProvider.GetRequiredService<IPublishersService>();

        var publisherExists = await publishersService.UserExists(@event.Id.ToString());
        if (!publisherExists)
        {
            publishersService.Add(new Publisher
            {
                UserId = @event.Id.ToString(),
                Name = @event.Name,
                PhoneNumber = @event.PhoneNumber,
            });
            await publishersService.Save();
        }

        return default;
    }

    public override async Task AfterActionApplierAsync(
        UserCreatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}