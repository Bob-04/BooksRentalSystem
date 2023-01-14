using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Projections.Services;

public interface IPublishersService
{
    void Add(Publisher publisher);

    Task Save(params object[] messages);
}

public class PublishersService : IPublishersService
{
    private readonly DbContext _data;

    public PublishersService(DbContext data)
    {
        _data = data;
    }

    public void Add(Publisher publisher)
    {
        _data.Add(publisher);
    }

    public async Task Save(params object[] messages)
    {
        var dataMessages = messages
            .ToDictionary(data => data, data => new Message(data));

        if (_data is MessageDbContext)
        {
            foreach (var (_, message) in dataMessages)
            {
                _data.Add(message);
            }
        }

        await _data.SaveChangesAsync();

        // if (_data is MessageDbContext)
        // {
        //     foreach (var (data, message) in dataMessages)
        //     {
        //         await _publisher.Publish(data);
        //
        //         message.MarkAsPublished();
        //
        //         await _data.SaveChangesAsync();
        //     }
        // }
    }
}