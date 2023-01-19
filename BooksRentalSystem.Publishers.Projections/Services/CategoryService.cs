using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Projections.Services;

public interface ICategoryService
{
    void Add(Category category);

    Task<Category?> Find(int categoryId);

    Task Save(params object[] messages);
}

public class CategoryService : ICategoryService
{
    private readonly DbContext _data;

    public CategoryService(DbContext data)
    {
        _data = data;
    }

    private IQueryable<Category> All() => _data.Set<Category>();

    public void Add(Category category)
    {
        _data.Add(category);
    }

    public async Task<Category?> Find(int categoryId)
    {
        return await _data.FindAsync<Category>(categoryId);
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
