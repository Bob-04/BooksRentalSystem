using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly DbContext _data;
        private readonly IMapper _mapper;

        public CategoryService(DbContext data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        private IQueryable<Category> All() => _data.Set<Category>();

        public void Add(Category category)
        {
            _data.Add(category);
        }

        public async Task<IEnumerable<CategoryOutputModel>> GetAll()
        {
            return await _mapper.ProjectTo<CategoryOutputModel>(All())
                .ToListAsync();
        }
    }
}
