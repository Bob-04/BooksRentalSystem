using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Projections.Services;

public interface IBookAdsService
{
    void Add(BookAd bookAd);

    Task<BookAd?> Find(int id);

    Task<bool> Delete(int id);

    Task Save(params object[] messages);
}

public class BookAdsService : IBookAdsService
{
    private const int BookAdsPerPage = 1000;

    private readonly DbContext _data;

    public BookAdsService(DbContext data)
    {
        _data = data;
    }

    private IQueryable<BookAd> All() => _data.Set<BookAd>();

    public void Add(BookAd bookAd)
    {
        _data.Add(bookAd);
    }

    public async Task<BookAd?> Find(int id)
    {
        return await All()
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> Delete(int id)
    {
        var bookAd = await _data.FindAsync<BookAd>(id);
        if (bookAd == null)
        {
            return false;
        }

        _data.Remove(bookAd);

        await _data.SaveChangesAsync();

        return true;
    }

    public async Task Save(params object[] messages)
    {
        var dataMessages = messages.ToDictionary(data => data, data => new Message(data));

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