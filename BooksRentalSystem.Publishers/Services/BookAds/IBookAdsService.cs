using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.BookAds;

namespace BooksRentalSystem.Publishers.Services.BookAds
{
    public interface IBookAdsService
    {
        void Add(BookAd bookAd);

        Task<BookAd> Find(int id);

        Task<bool> Delete(int id);

        Task<IEnumerable<BookAdOutputModel>> GetListings(BookAdsQuery query);

        Task<IEnumerable<MineBookAdOutputModel>> Mine(int publisherId, BookAdsQuery query);

        Task<BookAdDetailsOutputModel> GetDetails(int id);

        Task<int> Total(BookAdsQuery query);

        Task Save(params object[] messages);
    }
}
