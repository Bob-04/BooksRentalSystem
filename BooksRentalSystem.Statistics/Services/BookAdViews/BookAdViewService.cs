using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksRentalSystem.Statistics.Data.Models;
using BooksRentalSystem.Statistics.Models.BookAdViews;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Statistics.Services.BookAdViews
{
    public class BookAdViewService : IBookAdViewService
    {
        private readonly DbContext _data;

        public BookAdViewService(DbContext data)
        {
            _data = data;
        }

        private IQueryable<BookAdView> All() => _data.Set<BookAdView>();

        public async Task<int> GetTotalViews(int bookAdId)
        {
            return await All()
                .CountAsync(v => v.BookAdId == bookAdId);
        }

        public async Task<IEnumerable<BookAdViewOutputModel>> GetTotalViews(
            IEnumerable<int> ids)
        {
            return await All()
                .Where(v => ids.Contains(v.BookAdId))
                .GroupBy(v => v.BookAdId)
                .Select(gr => new BookAdViewOutputModel
                {
                    BookAdId = gr.Key,
                    TotalViews = gr.Count()
                })
                .ToListAsync();
        }
    }
}
