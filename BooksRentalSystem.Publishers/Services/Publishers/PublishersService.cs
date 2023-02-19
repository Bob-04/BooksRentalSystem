using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Common.Services.Messages;
using BooksRentalSystem.Publishers.Models.Publishers;
using Microsoft.EntityFrameworkCore;
using Publisher = BooksRentalSystem.Publishers.Data.Models.Publisher;

namespace BooksRentalSystem.Publishers.Services.Publishers
{
    public class PublishersService : IPublishersService
    {
        private readonly DbContext _data;
        private readonly IPublisher _publisher;
        private readonly IMapper _mapper;

        public PublishersService(DbContext data, IPublisher publisher, IMapper mapper)
        {
            _data = data;
            _publisher = publisher;
            _mapper = mapper;
        }

        private IQueryable<Publisher> All() => _data.Set<Publisher>();

        public void Add(Publisher publisher)
        {
            _data.Add(publisher);
        }

        public async Task<Publisher> FindById(int id)
        {
            return await _data.FindAsync<Publisher>(id);
        }

        public Task<Publisher> FindByUser(string userId)
        {
            return FindByUser(userId, publisher => publisher);
        }

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

        public async Task<PublisherOutputModel> GetDetailsByBookId(Guid bookAdId)
        {
            return await _mapper.ProjectTo<PublisherOutputModel>(
                    All().Where(p => p.BookAds.Any(b => b.AggregateId == bookAdId)))
                .SingleOrDefaultAsync();
        }

        public async Task Save(params object[] messages)
        {
            var dataMessages = messages
                .ToDictionary(data => data, data => new Message(data));

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
