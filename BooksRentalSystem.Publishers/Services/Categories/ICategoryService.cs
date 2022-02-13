using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.Categories;

namespace BooksRentalSystem.Publishers.Services.Categories
{
    public interface ICategoryService
    {
        void Add(Category category);

        Task<Category> Find(int categoryId);

        Task<IEnumerable<CategoryOutputModel>> GetAll();

        Task Save(params object[] messages);
    }
}
