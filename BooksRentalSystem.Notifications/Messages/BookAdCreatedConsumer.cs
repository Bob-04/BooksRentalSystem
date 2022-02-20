using System.Threading.Tasks;
using BooksRentalSystem.Common.Messages.Publishers;
using BooksRentalSystem.Notifications.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace BooksRentalSystem.Notifications.Messages
{
    public class BookAdCreatedConsumer : IConsumer<BookAdCreatedMessage>
    {
        private readonly IHubContext<NotificationsHub> _hub;

        public BookAdCreatedConsumer(IHubContext<NotificationsHub> hub)
        {
            _hub = hub;
        }

        public async Task Consume(ConsumeContext<BookAdCreatedMessage> context)
        {
            await _hub.Clients
                .Groups(NotificationsConstants.AuthenticatedUsersGroup)
                .SendAsync(NotificationsConstants.ReceiveNotificationEndpoint, context.Message);
        }
    }
}
