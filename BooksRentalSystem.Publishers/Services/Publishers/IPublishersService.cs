using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Models.Publishers;

namespace BooksRentalSystem.Publishers.Services.Publishers
{
    public interface IPublishersService
    {
        Task<int> GetIdByUser(string userId);

        Task<bool> HasBookAd(int publisherId, Guid bookAdId);

        Task<bool> IsPublisher(string userId);

        Task<IEnumerable<PublisherDetailsOutputModel>> GetAll();

        Task<PublisherDetailsOutputModel> GetDetails(int id);

        Task<PublisherDetailsOutputModel> GetDetails(Guid userId);
    }
}
