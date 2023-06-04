using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Models.Categories;

namespace BooksRentalSystem.Publishers.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryOutputModel>> GetAll();
    }
}
