using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BooksRentalSystem.Notifications.Infrastructure
{
    public class JwtConfiguration
    {
        public static JwtBearerEvents BearerEvents => new()
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notifications"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    }
}
