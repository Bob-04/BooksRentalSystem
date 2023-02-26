using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.BookAds;

namespace BooksRentalSystem.Publishers.Services.BookAds
{
    public interface IBookAdsService
    {
        Task<IEnumerable<BookAdOutputModel>> GetListings(BookAdsQuery query);

        Task<IEnumerable<MineBookAdOutputModel>> Mine(int publisherId, BookAdsQuery query);

        Task<BookAdDetailsOutputModel> GetDetails(Guid id);

        Task<int> Total(BookAdsQuery query);
    }
}
