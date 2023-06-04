using System;
using System.Threading.Tasks;
using BooksRentalSystem.Identity.Models.Identity;
using BooksRentalSystem.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Identity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
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

            return await Login(new UserLoginModel
            {
                Email = input.Email,
                Password = input.Password
            });
        }

        [HttpPost]
        [Route(nameof(Login))]
        public async Task<ActionResult<UserOutputModel>> Login(UserLoginModel input)
        {
            var result = await _identityService.Login(input);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return result.Data;
        }

        [HttpPut]
        [Route(nameof(ChangePassword) + "/{id:guid}")]
        public async Task<ActionResult> ChangePassword(Guid id, ChangePasswordInputModel input)
        {
            return await _identityService.ChangePassword(id.ToString(), new ChangePasswordInputModel
            {
                CurrentPassword = input.CurrentPassword,
                NewPassword = input.NewPassword
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserOutputModel>> EditUser(Guid id, EditUserInputModel input)
        {
            var result = await _identityService.EditUser(id, input);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Accepted();
        }
    }
}
