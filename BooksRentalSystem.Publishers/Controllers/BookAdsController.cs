using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Models;
using BooksRentalSystem.Common.Services.Identity;
using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Domain;
using BooksRentalSystem.Publishers.Models.BookAds;
using BooksRentalSystem.Publishers.Models.Categories;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventStoreAggregateRepository _eventStoreAggregateRepository;

        public BookAdsController(IBookAdsService bookAdsService, IPublishersService publishersService,
            ICategoryService categoryService, ICurrentUserService currentUserService,
            IEventStoreAggregateRepository eventStoreAggregateRepository)
        {
            _bookAdsService = bookAdsService;
            _publishersService = publishersService;
            _categoryService = categoryService;
            _currentUserService = currentUserService;
            _eventStoreAggregateRepository = eventStoreAggregateRepository;
        }

        [HttpGet]
        public async Task<ActionResult<SearchBookAdsOutputModel>> Search([FromQuery] BookAdsQuery query)
        {
            var bookAdListings = await _bookAdsService.GetListings(query);

            var totalBookAds = await _bookAdsService.Total(query);

            return new SearchBookAdsOutputModel(bookAdListings, query.Page, totalBookAds);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BookAdDetailsOutputModel>> Details(Guid id)
        {
            // return await _bookAdsService.GetDetails(id); // TO USE READ-MODEL

            var bookAdAggregate = await _eventStoreAggregateRepository.LoadAsync<BookAdAggregate>(id);
            if (bookAdAggregate.Id == default)
                return NotFound();

            var publisher = await _publishersService.GetDetails(bookAdAggregate.PublisherId);

            return new BookAdDetailsOutputModel
            {
                Id = bookAdAggregate.Id,
                Title = bookAdAggregate.Title,
                Description = bookAdAggregate.Description,
                Author = bookAdAggregate.AuthorName,
                ImageUrl = bookAdAggregate.ImageUrl,
                Category = bookAdAggregate.CategoryId,
                PricePerDay = bookAdAggregate.PricePerDay,
                PagesNumber = bookAdAggregate.PagesNumber,
                Language = bookAdAggregate.Language,
                PublicationDate = bookAdAggregate.PublicationDate,
                CoverType = (CoverType?)bookAdAggregate.CoverType,
                Publisher = publisher
            };
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(BookAdInputModel input)
        {
            var bookAdId = Guid.NewGuid();
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var bookAdAggregate = new BookAdAggregate { Id = bookAdId };
            bookAdAggregate.CreateBookAd(
                bookAdId,
                input.Title,
                input.Description,
                input.ImageUrl,
                input.PricePerDay,
                input.PagesNumber,
                input.Language,
                input.PublicationDate,
                input.CoverType,
                publisherId,
                input.Author,
                input.Category
            );

            await _eventStoreAggregateRepository.SaveAsync(bookAdAggregate);

            return Result.Success;
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Edit(Guid id, BookAdInputModel input)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var publisherHasBook = await _publishersService.HasBookAd(publisherId, id);
            if (!publisherHasBook)
                return BadRequest(Result.Failure("You cannot edit this book ad."));

            var bookAdAggregate = await _eventStoreAggregateRepository.LoadAsync<BookAdAggregate>(id);
            if (bookAdAggregate.Id == default)
                return NotFound();

            bookAdAggregate.UpdateBookAd(
                id,
                input.Title,
                input.Description,
                input.ImageUrl,
                input.PricePerDay,
                input.PagesNumber,
                input.Language,
                input.PublicationDate,
                input.CoverType,
                publisherId,
                input.Author,
                input.Category
            );
            await _eventStoreAggregateRepository.SaveAsync(bookAdAggregate);

            return Result.Success;
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<ActionResult> Delete(Guid id)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var publisherHasBook = await _publishersService.HasBookAd(publisherId, id);
            if (!publisherHasBook)
                return BadRequest(Result.Failure("You cannot delete this book ad."));

            var bookAdAggregate = await _eventStoreAggregateRepository.LoadAsync<BookAdAggregate>(id);
            if (bookAdAggregate.Id == default)
                return NotFound();

            bookAdAggregate.DeleteBookAd(id);
            await _eventStoreAggregateRepository.SaveAsync(bookAdAggregate);

            return Result.Success;
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

        [HttpPut("{id:guid}/" + nameof(ChangeAvailability))]
        [Authorize]
        public async Task<ActionResult> ChangeAvailability(Guid id, [FromQuery] bool available)
        {
            var publisherId = await _publishersService.GetIdByUser(_currentUserService.UserId);

            var publisherHasBook = await _publishersService.HasBookAd(publisherId, id);
            if (!publisherHasBook)
                return BadRequest(Result.Failure("You cannot delete this book ad."));

            var bookAdAggregate = await _eventStoreAggregateRepository.LoadAsync<BookAdAggregate>(id);
            if (bookAdAggregate.Id == default)
                return NotFound();

            bookAdAggregate.ChangeAvailability(id, available);
            await _eventStoreAggregateRepository.SaveAsync(bookAdAggregate);

            return Result.Success;
        }
    }
}
