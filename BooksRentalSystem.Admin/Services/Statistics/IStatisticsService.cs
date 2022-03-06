using System.Threading.Tasks;
using BooksRentalSystem.Admin.Models.Statistics;
using Refit;

namespace BooksRentalSystem.Admin.Services.Statistics
{
    public interface IStatisticsService
    {
        [Get("/Statistics")]
        Task<StatisticsOutputModel> Full();
    }
}
