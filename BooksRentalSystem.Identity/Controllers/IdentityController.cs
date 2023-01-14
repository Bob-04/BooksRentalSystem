using System.Threading.Tasks;
using BooksRentalSystem.Common.Services.Identity;
using BooksRentalSystem.Identity.Models.Identity;
using BooksRentalSystem.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Identity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;

        public IdentityController(IIdentityService identityService, ICurrentUserService currentUserService)
        {
            _identityService = identityService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Route(nameof(Register))]
        public async Task<ActionResult<UserOutputModel>> Register(UserInputModel input)
        {
            var result = await _identityService.Register(input);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return await Login(input);
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<UserOutputModel>> Login(UserInputModel input)
        {
            var result = await _identityService.Login(input);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }

        [HttpPut]
        [Authorize]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ChangePasswordInputModel input)
        {
            return await _identityService.ChangePassword(_currentUserService.UserId, new ChangePasswordInputModel
            {
                CurrentPassword = input.CurrentPassword,
                NewPassword = input.NewPassword
            });
        }
    }
}
