using System;
using BooksRentalSystem.Admin.Middlewares;
using BooksRentalSystem.Admin.Services;
using BooksRentalSystem.Admin.Services.Identity;
using BooksRentalSystem.Admin.Services.Publishers;
using BooksRentalSystem.Admin.Services.Statistics;
using BooksRentalSystem.Common.Extensions;
using BooksRentalSystem.Common.Services.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Refit;

namespace BooksRentalSystem.Admin
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
            var serviceEndpoints = Configuration
                .GetSection(nameof(ServiceEndpoints))
                .Get<ServiceEndpoints>(config => config.BindNonPublicProperties = true);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTokenAuthentication(Configuration);

            services.AddHealth(Configuration, databaseHealthChecks: false);

            services.AddScoped<ICurrentTokenService, CurrentTokenService>();

            services.AddTransient<JwtCookieAuthenticationMiddleware>();

            services.AddControllersWithViews(options => options
                .Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            services
                .AddRefitClient<IIdentityService>()
                .WithConfiguration(serviceEndpoints.Identity);

            services
                .AddRefitClient<IStatisticsService>()
                .WithConfiguration(serviceEndpoints.Statistics);

            services
                .AddRefitClient<IPublishersService>()
                .WithConfiguration(serviceEndpoints.Publishers);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseMiddleware<JwtCookieAuthenticationMiddleware>();

            app
                .UseAuthentication()
                .UseAuthorization();

            app.UseEndpoints(endpoints => endpoints
                .MapHealthChecks()
                .MapDefaultControllerRoute());
        }
    }
}
