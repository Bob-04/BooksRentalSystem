using System;
using Anabasis.InMemory.Extensions;
using BooksRentalSystem.Common.Extensions;
using BooksRentalSystem.Common.Services.Data;
using BooksRentalSystem.EventSourcing.Extensions;
using BooksRentalSystem.Publishers.Data;
using BooksRentalSystem.Publishers.Data.Seeding;
using BooksRentalSystem.Publishers.Services.Authors;
using BooksRentalSystem.Publishers.Services.BookAds;
using BooksRentalSystem.Publishers.Services.Categories;
using BooksRentalSystem.Publishers.Services.Publishers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Publishers
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
                .AddTransient<IDataSeeder, PublishersDataSeeder>()
                .AddTransient<IPublishersService, PublishersService>()
                .AddTransient<ICategoryService, CategoryService>()
                .AddTransient<IBookAdsService, BookAdsService>()
                .AddTransient<IAuthorsService, AuthorsService>();

            services
                .AddEventSourcing(Configuration.GetConnectionString("EventStore") ??
                                  throw new InvalidOperationException())
                .AddInMemorySnapshotsStore();

            services.AddMessaging(Configuration);
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
