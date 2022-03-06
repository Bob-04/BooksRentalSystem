using System;
using System.Threading.Tasks;
using AutoMapper;
using BooksRentalSystem.Admin.Models.Identity;
using BooksRentalSystem.Admin.Services.Identity;
using BooksRentalSystem.Common;
using BooksRentalSystem.Common.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksRentalSystem.Admin.Controllers
{
    public class IdentityController : AdministrationController
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public IdentityController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginFormModel model)
        {
            return await Handle(
                async () =>
                {
                    var result = await _identityService
                        .Login(_mapper.Map<UserInputModel>(model));

                    Response.Cookies.Append(
                        InfrastructureConstants.AuthenticationCookieName,
                        result.Token,
                        new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = false,
                            MaxAge = TimeSpan.FromDays(1)
                        });
                },
                success: RedirectToAction(nameof(StatisticsController.Index), "Statistics"),
                failure: View("../Home/Index", model));
        }

        [AuthorizeAdministrator]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(InfrastructureConstants.AuthenticationCookieName);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
