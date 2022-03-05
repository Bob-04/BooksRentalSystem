using System.Threading.Tasks;
using BooksRentalSystem.Statistics.Models.Statistics;
using BooksRentalSystem.Statistics.Services.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Statistics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<StatisticsOutputModel> Full()
        {
            return await _statisticsService.Full();
        }
    }
}
