using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Common.Data.Models;
using BooksRentalSystem.Common.Services.Messages;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly DbContext _data;
        private readonly IPublisher _publisher;
        private readonly IMapper _mapper;

        public CategoryService(DbContext data, IPublisher publisher, IMapper mapper)
        {
            _data = data;
            _publisher = publisher;
            _mapper = mapper;
        }

        private IQueryable<Category> All() => _data.Set<Category>();

        public void Add(Category category)
        {
            _data.Add(category);
        }

        public async Task<Category> Find(int categoryId)
        {
            return await _data.FindAsync<Category>(categoryId);
        }

        public async Task<IEnumerable<CategoryOutputModel>> GetAll()
        {
            return await _mapper.ProjectTo<CategoryOutputModel>(All())
                .ToListAsync();
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
    }
}
