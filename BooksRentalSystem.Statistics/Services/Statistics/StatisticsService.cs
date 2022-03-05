using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Statistics.Models.Statistics;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Statistics.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly DbContext _data;
        private readonly IMapper _mapper;

        public StatisticsService(DbContext data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        private IQueryable<Data.Models.Statistics> All() => _data.Set<Data.Models.Statistics>();

        public async Task<StatisticsOutputModel> Full()
        {
            return await _mapper
                .ProjectTo<StatisticsOutputModel>(All())
                .SingleOrDefaultAsync();
        }
    }
}
