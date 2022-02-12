using System;
using System.Text;
using BooksRentalSystem.Common.Services.Identity;
using BooksRentalSystem.Common.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BooksRentalSystem.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebService<TDbContext>(this IServiceCollection services,
            IConfiguration configuration, bool databaseHealthChecks = true, bool messagingHealthChecks = true)
            where TDbContext : DbContext
        {
            services
                .AddDatabase<TDbContext>(configuration)
                .AddApplicationSettings(configuration)
                .AddTokenAuthentication(configuration)
                .AddHealth(configuration, databaseHealthChecks, messagingHealthChecks)
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddControllers();

            return services;
        }

        public static IServiceCollection AddDatabase<TDbContext>(this IServiceCollection services,
            IConfiguration configuration)
            where TDbContext : DbContext
        {
            return services
                .AddScoped<DbContext, TDbContext>()
                .AddDbContext<TDbContext>(options => options.UseSqlServer(
                    configuration.GetDefaultConnectionString(),
                    sqlOptions => sqlOptions
                        .EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null)));
        }

        public static IServiceCollection AddApplicationSettings(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .Configure<ApplicationSettings>(
                    configuration.GetSection(nameof(ApplicationSettings)),
                    config => config.BindNonPublicProperties = true);
        }

        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services,
            IConfiguration configuration, JwtBearerEvents events = null)
        {
            services
                .AddHttpContextAccessor()
                .AddScoped<ICurrentUserService, CurrentUserService>();

            var secret = configuration
                .GetSection(nameof(ApplicationSettings))
                .GetValue<string>(nameof(ApplicationSettings.Secret));

            var key = Encoding.ASCII.GetBytes(secret);

            services
                .AddAuthentication(authentication =>
                {
                    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(bearer =>
                {
                    bearer.RequireHttpsMetadata = false;
                    bearer.SaveToken = true;
                    bearer.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };

                    if (events != null)
                    {
                        bearer.Events = events;
                    }
                });

            return services;
        }

        public static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration,
            bool databaseHealthChecks = true, bool messagingHealthChecks = true)
        {
            var healthChecks = services.AddHealthChecks();

            if (databaseHealthChecks)
            {
                healthChecks.AddSqlServer(configuration.GetDefaultConnectionString());
            }

            if (messagingHealthChecks)
            {
                var messageQueueSettings = GetMessageQueueSettings(configuration);

                var messageQueueConnectionString =
                    $"amqp://{messageQueueSettings.UserName}:{messageQueueSettings.Password}@{messageQueueSettings.Host}/";

                healthChecks.AddRabbitMQ(rabbitConnectionString: messageQueueConnectionString);
            }

            return services;
        }

        private static MessageQueueSettings GetMessageQueueSettings(IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(MessageQueueSettings));

            return new MessageQueueSettings(
                settings.GetValue<string>(nameof(MessageQueueSettings.Host)),
                settings.GetValue<string>(nameof(MessageQueueSettings.UserName)),
                settings.GetValue<string>(nameof(MessageQueueSettings.Password)));
        }
    }
}
