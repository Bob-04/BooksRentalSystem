using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Data.Models;

namespace BooksRentalSystem.Publishers.Services.Authors
{
    public interface IAuthorsService
    {
        void Add(Author author);

        Task<Author> FindByName(string name);

        Task Save(params object[] messages);
    }
}
