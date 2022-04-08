using System.Threading.Tasks;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Common.Messages.Publishers;
using BooksRentalSystem.Statistics.Data;
using BooksRentalSystem.Statistics.Services.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Statistics.Messages
{
    public class BookAdCreatedConsumer : IConsumer<BookAdCreatedMessage>
    {
        private readonly ApplicationDbContext _data;
        private readonly IMessageService _messages;

        public BookAdCreatedConsumer(ApplicationDbContext data, IMessageService messages)
        {
            _data = data;
            _messages = messages;
        }

        public async Task Consume(ConsumeContext<BookAdCreatedMessage> context)
        {
            var message = context.Message;

            var isDuplicated = await _messages.IsDuplicated(
                message,
                nameof(BookAdCreatedMessage.BookAdId),
                message.BookAdId);

            if (!isDuplicated)
            {
                var statistics = await _data.Statistics.SingleOrDefaultAsync();
                statistics.TotalBookAds++;

                var dataMessage = new Message(message);
                dataMessage.MarkAsPublished();
                _data.Messages.Add(dataMessage);

                await _data.SaveChangesAsync();
            }
        }
    }
}
