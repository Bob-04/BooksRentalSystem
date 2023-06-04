using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Projections.Services;

public interface IAuthorsService
{
    void Add(Author author);

    Task<Author?> FindByName(string name);

    Task Save(params object[] messages);
}

public class AuthorsService : IAuthorsService
{
    private readonly DbContext _data;

    public AuthorsService(DbContext data)
    {
        _data = data;
    }

    private IQueryable<Author> All() => _data.Set<Author>();

    public void Add(Author author)
    {
        _data.Add(author);
    }

    public async Task<Author?> FindByName(string name)
    {
        return await All()
            .FirstOrDefaultAsync(a => a.Name == name);
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
