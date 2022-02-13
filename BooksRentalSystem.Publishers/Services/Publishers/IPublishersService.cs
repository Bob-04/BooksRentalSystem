using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.Publishers;

namespace BooksRentalSystem.Publishers.Services.Publishers
{
    public interface IPublishersService
    {
        void Add(Publisher publisher);

        Task<Publisher> FindById(int id);

        Task<Publisher> FindByUser(string userId);

        Task<int> GetIdByUser(string userId);

        Task<bool> HasBookAd(int publisherId, int bookAdId);

        Task<bool> IsPublisher(string userId);

        Task<IEnumerable<PublisherDetailsOutputModel>> GetAll();

        Task<PublisherDetailsOutputModel> GetDetails(int publisherId);

        Task<PublisherOutputModel> GetDetailsByBookId(int bookAdId);

        Task Save(params object[] messages);
    }
}
