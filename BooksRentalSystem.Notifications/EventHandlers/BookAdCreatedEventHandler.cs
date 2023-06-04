using System;
using System.Threading;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Messages.Publishers;
using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Notifications.Hubs;
using BooksRentalSystem.Publishers.Domain.Events;
using EventStore.Client;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Notifications.EventHandlers;

public class BookAdCreatedEventHandler : EventStoreEventHandler<BookAdCreatedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public BookAdCreatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        BookAdCreatedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var hub = serviceScope.ServiceProvider.GetRequiredService<IHubContext<NotificationsHub>>();

        await hub.Clients
            .Groups(NotificationsConstants.AuthenticatedUsersGroup)
            .SendAsync(
                NotificationsConstants.ReceiveNotificationEndpoint, new BookAdCreatedMessage
                {
                    BookAdId = @event.Id,
                    Title = @event.Title,
                    Author = @event.AuthorName,
                    PricePerDay = @event.PricePerDay
                },
                cancellationToken: cancellationToken
            );

        return default;
    }

    public override async Task AfterActionApplierAsync(
        BookAdCreatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}