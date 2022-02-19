using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Common.Services.Messages;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BooksRentalSystem.Common.Messages
{
    public class MessagesHostedService : IHostedService
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IPublisher _publisher;

        public MessagesHostedService(IRecurringJobManager recurringJobManager, IServiceScopeFactory serviceScopeFactory,
            IPublisher publisher)
        {
            _recurringJobManager = recurringJobManager;
            _serviceScopeFactory = serviceScopeFactory;
            _publisher = publisher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var data = scope.ServiceProvider.GetService<DbContext>();
            if (data == null)
            {
                throw new NullReferenceException("data is null");
            }

            if (!data.Database.CanConnect())
            {
                data.Database.Migrate();
            }

            _recurringJobManager.AddOrUpdate(
                nameof(MessagesHostedService),
                () => ProcessPendingMessages(),
                "*/5 * * * * *");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void ProcessPendingMessages()
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var data = scope.ServiceProvider.GetService<DbContext>();
            if (data == null)
            {
                throw new NullReferenceException("data is null");
            }

            var messages = data
                .Set<Message>()
                .Where(m => !m.Published)
                .OrderBy(m => m.Id)
                .ToList();

            foreach (var message in messages)
            {
                _publisher
                    .Publish(message.Data, message.Type)
                    .GetAwaiter()
                    .GetResult();

                message.MarkAsPublished();

                data.SaveChanges();
            }
        }
    }
}
