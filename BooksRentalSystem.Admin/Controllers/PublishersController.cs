using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Admin.Models.Publishers;
using BooksRentalSystem.Admin.Services.Publishers;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Admin.Controllers
{
    public class PublishersController : AdministrationController
    {
        private readonly IPublishersService _publishersService;
        private readonly IMapper _mapper;

        public PublishersController(IPublishersService publishersService, IMapper mapper)
        {
            _publishersService = publishersService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _publishersService.All());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var publisher = await _publishersService.Details(id);

            var publisherForm = _mapper.Map<PublisherFormModel>(publisher);

            return View(publisherForm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, PublisherFormModel model)
        {
            return await Handle(
                async () => await _publishersService
                    .Edit(id, _mapper.Map<PublisherInputModel>(model)),
                success: RedirectToAction(nameof(Index)),
                failure: View(model));
        }
    }
}
