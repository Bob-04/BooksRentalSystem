using System.Threading.Tasks;
using BooksRentalSystem.Statistics.Models.Statistics;

namespace BooksRentalSystem.Statistics.Services.Statistics
{
    public interface IStatisticsService
    {
        Task<StatisticsOutputModel> Full();
    }
}
