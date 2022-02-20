using BooksRentalSystem.Common.Extensions;
using BooksRentalSystem.Notifications.Hubs;
using BooksRentalSystem.Notifications.Infrastructure;
using BooksRentalSystem.Notifications.Messages;
using BooksRentalSystem.Notifications.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BooksRentalSystem.Notifications
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddTokenAuthentication(Configuration, JwtConfiguration.BearerEvents);

            services.AddHealth(Configuration, databaseHealthChecks: false);

            services.AddMessaging(Configuration, usePolling: false, consumers: typeof(BookAdCreatedConsumer));

            services.AddSignalR();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var allowedOrigins = Configuration
                .GetSection(nameof(NotificationSettings))
                .GetValue<string>(nameof(NotificationSettings.AllowedOrigins));

            app.UseRouting();

            app.UseCors(options => options
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints
                .MapHealthChecks()
                .MapHub<NotificationsHub>("/notifications"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
