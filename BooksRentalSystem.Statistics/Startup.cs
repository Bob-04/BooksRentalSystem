using BooksRentalSystem.Common.Extensions;
using BooksRentalSystem.Common.Services.Data;
using BooksRentalSystem.Statistics.Data;
using BooksRentalSystem.Statistics.Data.Seeding;
using BooksRentalSystem.Statistics.Messages;
using BooksRentalSystem.Statistics.Services.BookAdViews;
using BooksRentalSystem.Statistics.Services.Messages;
using BooksRentalSystem.Statistics.Services.Statistics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Statistics
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
            services.AddWebService<ApplicationDbContext>(Configuration);

            services
                .AddTransient<IDataSeeder, StatisticsDataSeeder>()
                .AddTransient<IStatisticsService, StatisticsService>()
                .AddTransient<IBookAdViewService, BookAdViewService>()
                .AddTransient<IMessageService, MessageService>();

            services.AddMessaging(Configuration, consumers: typeof(BookAdCreatedConsumer));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebService(env);

            app.Initialize();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}
