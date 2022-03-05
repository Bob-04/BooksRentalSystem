using AutoMapper;
using BooksRentalSystem.Statistics.Models.Statistics;

namespace BooksRentalSystem.Statistics.Mapping
{
    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Data.Models.Statistics, StatisticsOutputModel>();
        }
    }
}
