using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Messages.Publishers;
using BooksRentalSystem.Common.Models;
using BooksRentalSystem.Common.Services.Identity;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.BookAds;
using BooksRentalSystem.Publishers.Models.Categories;
using BooksRentalSystem.Publishers.Services.Authors;
using BooksRentalSystem.Publishers.Services.BookAds;
using BooksRentalSystem.Publishers.Services.Categories;
using BooksRentalSystem.Publishers.Services.Publishers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Publishers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookAdsController : ControllerBase
    {
        private readonly IBookAdsService _bookAdsService;
        private readonly IPublishersService _publishersService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorsService _authorsService;
        private readonly ICurrentUserService _currentUserService;

        public BookAdsController(IBookAdsService bookAdsService, IPublishersService publishersService,
            ICategoryService categoryService, IAuthorsService authorsService, ICurrentUserService currentUserService)
        {
            _bookAdsService = bookAdsService;
            _publishersService = publishersService;
            _categoryService = categoryService;
            _authorsService = authorsService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<ActionResult<SearchBookAdsOutputModel>> Search([FromQuery] BookAdsQuery query)
        {
            var bookAdListings = await _bookAdsService.GetListings(query);

            var totalBookAds = await _bookAdsService.Total(query);

            return new SearchBookAdsOutputModel(bookAdListings, query.Page, totalBookAds);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookAdDetailsOutputModel>> Details(int id)
        {
            return await _bookAdsService.GetDetails(id);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CreateBookAdOutputModel>> Create(BookAdInputModel input)
        {
            var publisher = await _publishersService.FindByUser(_currentUserService.UserId);

            var category = await _categoryService.Find(input.Category);

            if (category == null)
            {
                return BadRequest(Result.Failure("Category does not exist."));
            }

            var author = await _authorsService.FindByName(input.Author);

            author ??= new Author
            {
                Name = input.Author
            };

            var bookAd = new BookAd
            {
                Title = input.Title,
                Description = input.Description,
                ImageUrl = input.ImageUrl,
                PricePerDay = input.PricePerDay,
                Publisher = publisher,
                Author = author,
                Category = category,
                BookInfo = new BookInfo
                {
                    PagesNumber = input.PagesNumber,
                    Language = input.Language,
                    PublicationDate = input.PublicationDate,
                    CoverType = input.Cover
                }
            };

            _bookAdsService.Add(bookAd);

            var message = new BookAdCreatedMessage
            {
                BookAdId = bookAd.Id,
                Title = bookAd.Title,
                Author = bookAd.Author.Name,
                PricePerDay = bookAd.PricePerDay
            };

            await _bookAdsService.Save(message);

            return new CreateBookAdOutputModel(bookAd.Id);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult> Edit(int id, BookAdInputModel input)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var publisherHasBook = await _publishersService.HasBookAd(publisherId, id);

            if (!publisherHasBook)
            {
                return BadRequest(Result.Failure("You cannot edit this book ad."));
            }

            var category = await _categoryService.Find(input.Category);

            var author = await _authorsService.FindByName(input.Author);

            author ??= new Author
            {
                Name = input.Author
            };

            var bookAd = await _bookAdsService.Find(id);

            bookAd.Title = input.Title;
            bookAd.Description = input.Description;
            bookAd.ImageUrl = input.ImageUrl;
            bookAd.PricePerDay = input.PricePerDay;
            bookAd.Author = author;
            bookAd.Category = category;
            bookAd.BookInfo = new BookInfo
            {
                PagesNumber = input.PagesNumber,
                Language = input.Language,
                PublicationDate = input.PublicationDate,
                CoverType = input.Cover
            };

            var message = new BookAdUpdatedMessage
            {
                BookAdId = bookAd.Id,
                Title = bookAd.Title,
                Author = bookAd.Author.Name
            };

            await _bookAdsService.Save(message);

            return Result.Success;
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var publisherHasBook = await _publishersService.HasBookAd(publisherId, id);

            if (!publisherHasBook)
            {
                return BadRequest(Result.Failure("You cannot delete this book ad."));
            }

            return await _bookAdsService.Delete(id);
        }

        [HttpGet(nameof(Mine))]
        [Authorize]
        public async Task<ActionResult<MineBookAdsOutputModel>> Mine([FromQuery] BookAdsQuery query)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var bookAdListings = await _bookAdsService.Mine(publisherId, query);

            var totalBookAds = await _bookAdsService.Total(query);

            return new MineBookAdsOutputModel(bookAdListings, query.Page, totalBookAds);
        }

        [HttpGet(nameof(Categories))]
        public async Task<IEnumerable<CategoryOutputModel>> Categories()
        {
            return await _categoryService.GetAll();
        }

        [HttpPut("{id:int}/" + nameof(ChangeAvailability))]
        [Authorize]
        public async Task<ActionResult> ChangeAvailability(int id)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var publisherHasBook = await _publishersService.HasBookAd(publisherId, id);
            if (!publisherHasBook)
            {
                return BadRequest(Result.Failure("You cannot edit this book ad."));
            }

            var bookAd = await _bookAdsService.Find(id);

            bookAd.IsAvailable = !bookAd.IsAvailable;

            await _bookAdsService.Save();

            return Result.Success;
        }
    }
}
