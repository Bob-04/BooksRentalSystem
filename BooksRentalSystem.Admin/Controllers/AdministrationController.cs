using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Refit;

namespace BooksRentalSystem.Admin.Controllers
{
    [AuthorizeAdministrator]
    public abstract class AdministrationController : Controller
    {
        protected async Task<ActionResult> Handle(Func<Task> action, ActionResult success, ActionResult failure)
        {
            try
            {
                await action();
                return success;
            }
            catch (ApiException exception)
            {
                ProcessErrors(exception);
                return failure;
            }
        }

        private void ProcessErrors(ApiException exception)
        {
            if (exception.HasContent)
            {
                JsonConvert
                    .DeserializeObject<List<string>>(exception.Content)
                    .ForEach(error => ModelState.AddModelError(string.Empty, error));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Internal server error.");
            }
        }
    }
}
