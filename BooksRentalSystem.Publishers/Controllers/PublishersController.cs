using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Attributes;
using BooksRentalSystem.Common.Models;
using BooksRentalSystem.Common.Services.Identity;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.Publishers;
using BooksRentalSystem.Publishers.Services.Publishers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Publishers.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublishersService _publishersService;
        private readonly ICurrentUserService _currentUserService;

        public PublishersController(IPublishersService publishersService, ICurrentUserService currentUserService)
        {
            _publishersService = publishersService;
            _currentUserService = currentUserService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PublisherDetailsOutputModel>> Details(int id)
        {
            return await _publishersService.GetDetails(id);
        }

        [HttpGet("id")]
        [Authorize]
        public async Task<ActionResult<int>> GetPublisherId()
        {
            var userId = _currentUserService.UserId;

            var userIsPublisher = await _publishersService.IsPublisher(userId);

            if (!userIsPublisher)
            {
                return BadRequest("This user is not a publisher.");
            }

            return await _publishersService.GetIdByUser(_currentUserService.UserId);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<int>> Create(CreatePublisherInputModel input)
        {
            var publisher = new Publisher
            {
                Name = input.Name,
                PhoneNumber = input.PhoneNumber,
                UserId = _currentUserService.UserId
            };

            _publishersService.Add(publisher);

            await _publishersService.Save();

            return publisher.Id;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Edit(int id, EditPublisherInputModel input)
        {
            var publisher = _currentUserService.IsAdministrator
                ? await _publishersService.FindById(id)
                : await _publishersService.FindByUser(_currentUserService.UserId);

            if (id != publisher.Id)
            {
                return BadRequest(Result.Failure("You cannot edit this publisher."));
            }

            publisher.Name = input.Name;
            publisher.PhoneNumber = input.PhoneNumber;

            await _publishersService.Save();

            return Ok();
        }

        [HttpGet]
        [AuthorizeAdministrator]
        public async Task<IEnumerable<PublisherDetailsOutputModel>> All()
        {
            return await _publishersService.GetAll();
        }
    }
}
