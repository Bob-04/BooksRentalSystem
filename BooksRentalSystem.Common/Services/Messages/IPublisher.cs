using System;
using System.Threading.Tasks;

namespace BooksRentalSystem.Common.Services.Messages
{
    public interface IPublisher
    {
        Task Publish<TMessage>(TMessage message);

        Task Publish<TMessage>(TMessage message, Type messageType);
    }
}
