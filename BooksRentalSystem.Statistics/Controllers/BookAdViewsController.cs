using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Statistics.Models.BookAdViews;
using BooksRentalSystem.Statistics.Services.BookAdViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Statistics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookAdViewsController : ControllerBase
    {
        private readonly IBookAdViewService _bookAdViewsService;

        public BookAdViewsController(IBookAdViewService bookAdViewsService)
        {
            _bookAdViewsService = bookAdViewsService;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<int> TotalViews(int id)
        {
            return await _bookAdViewsService.GetTotalViews(id);
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<BookAdViewOutputModel>> TotalViews([FromQuery] IEnumerable<int> ids)
        {
            return await _bookAdViewsService.GetTotalViews(ids);
        }
    }
}
