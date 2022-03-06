using System.Threading.Tasks;
using BooksRentalSystem.Admin.Services.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Admin.Controllers
{
    public class StatisticsController : AdministrationController
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _statisticsService.Full());
        }
    }
}
