﻿using System;
using System.Text;
using BooksRentalSystem.Common.Messages;
using BooksRentalSystem.Common.Services.Identity;
using BooksRentalSystem.Common.Services.Messages;
using BooksRentalSystem.Common.Settings;
using GreenPipes;
using Hangfire;
using Hangfire.SqlServer;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BooksRentalSystem.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebService<TDbContext>(this IServiceCollection services,
            IConfiguration configuration, bool databaseHealthChecks = true, bool messagingHealthChecks = false)
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
            bool databaseHealthChecks = true, bool messagingHealthChecks = false)
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

        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration,
            bool usePolling = true, params Type[] consumers)
        {
            services.AddTransient<IPublisher, Publisher>();

            var messageQueueSettings = GetMessageQueueSettings(configuration);

            services
                .AddMassTransit(mt =>
                {
                    foreach (var consumer in consumers)
                    {
                        mt.AddConsumer(consumer);
                    }

                    mt.AddBus(context => Bus.Factory.CreateUsingRabbitMq(rmq =>
                    {
                        rmq.Host(messageQueueSettings.Host, host =>
                        {
                            host.Username(messageQueueSettings.UserName);
                            host.Password(messageQueueSettings.Password);
                        });

                        rmq.UseHealthCheck(context);

                        foreach (var consumer in consumers)
                        {
                            rmq.ReceiveEndpoint(consumer.FullName, endpoint =>
                            {
                                endpoint.PrefetchCount = 6;
                                endpoint.UseMessageRetry(retry => retry.Interval(5, 200));

                                endpoint.ConfigureConsumer(context, consumer);
                            });
                        }
                    }));
                })
                .AddMassTransitHostedService();

            if (usePolling)
            {
                CreateHangfireDatabase(configuration);

                services
                    .AddHangfire(config => config
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(
                            configuration.GetCronJobsConnectionString(),
                            new SqlServerStorageOptions
                            {
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.Zero,
                                UseRecommendedIsolationLevel = true,
                                DisableGlobalLocks = true
                            }));

                services.AddHangfireServer();

                services.AddHostedService<MessagesHostedService>();
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

        private static void CreateHangfireDatabase(IConfiguration configuration)
        {
            var connectionString = configuration.GetCronJobsConnectionString();

            var dbName = connectionString
                .Split(";")[1]
                .Split("=")[1];

            using var connection = new SqlConnection(connectionString.Replace(dbName, "master"));

            connection.Open();

            using var command = new SqlCommand(
                $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{dbName}') create database [{dbName}];",
                connection);

            command.ExecuteNonQuery();
        }
    }
}
