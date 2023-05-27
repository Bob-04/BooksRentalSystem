using System;
using Anabasis.InMemory.Extensions;
using BooksRentalSystem.Common.Extensions;
using BooksRentalSystem.Common.Services.Data;
using BooksRentalSystem.EventSourcing.Extensions;
using BooksRentalSystem.Identity.Data;
using BooksRentalSystem.Identity.Data.Seeding;
using BooksRentalSystem.Identity.Extensions;
using BooksRentalSystem.Identity.Services;
using BooksRentalSystem.Identity.Settings;
using BooksRentalSystem.Snapshotting.MongoMemory.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace BooksRentalSystem.Identity
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
            services.Configure<IdentitySettings>(Configuration.GetSection(nameof(IdentitySettings)),
                config => config.BindNonPublicProperties = true);

            services.AddWebService<ApplicationDbContext>(Configuration);

            services.AddUsersStorage();

            services
                .AddEventSourcing(Configuration.GetConnectionString("EventStore") ??
                                  throw new InvalidOperationException())
                .AddMongoMemorySnapshotsStore(Configuration.GetConnectionString("MongoDb"));

            services
                .AddTransient<IDataSeeder, IdentityDataSeeder>()
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<ITokenGeneratorService, TokenGeneratorService>();
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
