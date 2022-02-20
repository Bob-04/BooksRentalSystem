using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BooksRentalSystem.Notifications.Hubs
{
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, NotificationsConstants.AuthenticatedUsersGroup);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User?.Identity?.IsAuthenticated == true)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, NotificationsConstants.AuthenticatedUsersGroup);
            }
        }
    }
}
