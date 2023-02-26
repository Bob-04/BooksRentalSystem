using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Publishers.Models.Publishers;
using Microsoft.EntityFrameworkCore;
using Publisher = BooksRentalSystem.Publishers.Data.Models.Publisher;

namespace BooksRentalSystem.Publishers.Services.Publishers
{
    public class PublishersService : IPublishersService
    {
        private readonly DbContext _data;
        private readonly IMapper _mapper;

        public PublishersService(DbContext data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        private IQueryable<Publisher> All() => _data.Set<Publisher>();

        public Task<int> GetIdByUser(string userId)
        {
            return FindByUser(userId, publisher => publisher.Id);
        }

        public async Task<bool> HasBookAd(int publisherId, Guid bookAdId)
        {
            return await All()
                .Where(p => p.Id == publisherId)
                .AnyAsync(p => p.BookAds.Any(b => b.AggregateId == bookAdId));
        }

        public async Task<bool> IsPublisher(string userId)
        {
            return await All()
                .AnyAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<PublisherDetailsOutputModel>> GetAll()
        {
            return await _mapper.ProjectTo<PublisherDetailsOutputModel>(All())
                .ToListAsync();
        }

        public async Task<PublisherDetailsOutputModel> GetDetails(Guid userId)
        {
            return await _mapper.ProjectTo<PublisherDetailsOutputModel>(
                    All().Where(p => p.UserId == userId.ToString()))
                .FirstOrDefaultAsync();
        }

        private async Task<T> FindByUser<T>(string userId, Expression<Func<Publisher, T>> selector)
        {
            var publisherData = await All()
                .Where(p => p.UserId == userId)
                .Select(selector)
                .FirstOrDefaultAsync();

            if (publisherData == null)
            {
                throw new InvalidOperationException("This user is not a publisher.");
            }

            return publisherData;
        }
    }
}
