using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Attributes;
using BooksRentalSystem.Common.Services.Identity;
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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PublisherDetailsOutputModel>> Details(Guid id)
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

        [HttpGet]
        [AuthorizeAdministrator]
        public async Task<IEnumerable<PublisherDetailsOutputModel>> All()
        {
            return await _publishersService.GetAll();
        }
    }
}
