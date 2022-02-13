using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Common.Services.Messages;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.BookAds;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Services.BookAds
{
    public class BookAdsService : IBookAdsService
    {
        private const int BookAdsPerPage = 1000;

        private readonly DbContext _data;
        private readonly IPublisher _publisher;
        private readonly IMapper _mapper;

        public BookAdsService(DbContext data, IPublisher publisher, IMapper mapper)
        {
            _data = data;
            _publisher = publisher;
            _mapper = mapper;
        }

        private IQueryable<BookAd> All() => _data.Set<BookAd>();

        public void Add(BookAd bookAd)
        {
            _data.Add(bookAd);
        }

        public async Task<BookAd> Find(int id)
        {
            return await All()
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> Delete(int id)
        {
            var bookAd = await _data.FindAsync<BookAd>(id);
            if (bookAd == null)
            {
                return false;
            }

            _data.Remove(bookAd);

            await _data.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<BookAdOutputModel>> GetListings(BookAdsQuery query)
        {
            return await _mapper.ProjectTo<BookAdOutputModel>(GetBookAdsQuery(query))
                .ToListAsync();
        }

        public async Task<IEnumerable<MineBookAdOutputModel>> Mine(int publisherId, BookAdsQuery query)
        {
            return await _mapper.ProjectTo<MineBookAdOutputModel>(GetBookAdsQuery(query, publisherId))
                .ToListAsync();
        }

        public async Task<BookAdDetailsOutputModel> GetDetails(int id)
        {
            return await _mapper.ProjectTo<BookAdDetailsOutputModel>(
                    AllAvailable().Where(b => b.Id == id))
                .FirstOrDefaultAsync();
        }

        public async Task<int> Total(BookAdsQuery query)
        {
            return await GetBookAdsQuery(query, includePaging: false)
                .CountAsync();
        }

        public async Task Save(params object[] messages)
        {
            var dataMessages = messages.ToDictionary(data => data, data => new Message(data));

            if (_data is MessageDbContext)
            {
                foreach (var (_, message) in dataMessages)
                {
                    _data.Add(message);
                }
            }

            await _data.SaveChangesAsync();

            if (_data is MessageDbContext)
            {
                foreach (var (data, message) in dataMessages)
                {
                    await _publisher.Publish(data);

                    message.MarkAsPublished();

                    await _data.SaveChangesAsync();
                }
            }
        }

        private IQueryable<BookAd> AllAvailable()
            => All().Where(b => b.IsAvailable);

        private IQueryable<BookAd> GetBookAdsQuery(BookAdsQuery query, int? authorId = null, bool includePaging = true)
        {
            var dataQuery = AllAvailable();

            if (authorId.HasValue)
            {
                dataQuery = All().Where(b => b.AuthorId == authorId);
            }

            if (query.Category.HasValue)
            {
                dataQuery = dataQuery.Where(b => b.CategoryId == query.Category);
            }

            if (!string.IsNullOrWhiteSpace(query.Author))
            {
                dataQuery = dataQuery.Where(b => b
                    .Author.Name.ToLower().Contains(query.Author.ToLower()));
            }

            if (query.MinPricePerDay.HasValue)
            {
                dataQuery = dataQuery.Where(b => b.PricePerDay >= query.MinPricePerDay);
            }

            if (query.MaxPricePerDay.HasValue)
            {
                dataQuery = dataQuery.Where(b => b.PricePerDay <= query.MaxPricePerDay);
            }

            if (includePaging)
            {
                dataQuery = dataQuery
                    .Skip((query.Page - 1) * BookAdsPerPage)
                    .Take(BookAdsPerPage);
            }

            return dataQuery;
        }
    }
}
