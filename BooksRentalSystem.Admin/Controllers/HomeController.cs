using System.Diagnostics;
using BooksRentalSystem.Admin.Models;
using BooksRentalSystem.Common.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsAdministrator())
            {
                return RedirectToAction(nameof(StatisticsController.Index), "Statistics");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
