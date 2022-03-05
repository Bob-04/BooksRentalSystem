using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Statistics.Models.BookAdViews;

namespace BooksRentalSystem.Statistics.Services.BookAdViews
{
    public interface IBookAdViewService
    {
        Task<int> GetTotalViews(int bookAdId);

        Task<IEnumerable<BookAdViewOutputModel>> GetTotalViews(IEnumerable<int> ids);
    }
}
