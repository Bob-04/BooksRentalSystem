using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.BookAds;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Services.BookAds
{
    public class BookAdsService : IBookAdsService
    {
        private const int BookAdsPerPage = 1000;

        private readonly DbContext _data;
        private readonly IMapper _mapper;

        public BookAdsService(DbContext data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        private IQueryable<BookAd> All() => _data.Set<BookAd>();

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

        public async Task<BookAdDetailsOutputModel> GetDetails(Guid id)
        {
            return await _mapper.ProjectTo<BookAdDetailsOutputModel>(
                    AllAvailable().Where(b => b.AggregateId == id))
                .FirstOrDefaultAsync();
        }

        public async Task<int> Total(BookAdsQuery query)
        {
            return await GetBookAdsQuery(query, includePaging: false)
                .CountAsync();
        }

        private IQueryable<BookAd> AllAvailable()
            => All().Where(b => b.IsAvailable);

        private IQueryable<BookAd> GetBookAdsQuery(BookAdsQuery query, int? publisherId = null, bool includePaging = true)
        {
            var dataQuery = AllAvailable();

            if (publisherId.HasValue)
            {
                dataQuery = All().Where(b => b.PublisherId == publisherId);
            }

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                dataQuery = dataQuery.Where(b => b
                    .Title.ToLower().Contains(query.Title.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.Author))
            {
                dataQuery = dataQuery.Where(b => b
                    .Author.Name.ToLower().Contains(query.Author.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(query.Publisher))
            {
                dataQuery = dataQuery.Where(b => b
                    .Publisher.Name.ToLower().Contains(query.Publisher.ToLower()));
            }

            if (query.Category.HasValue)
            {
                dataQuery = dataQuery.Where(b => b.CategoryId == query.Category);
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
