using System.Threading.Tasks;
using BooksRentalSystem.Admin.Models.Identity;
using Refit;

namespace BooksRentalSystem.Admin.Services.Identity
{
    public interface IIdentityService
    {
        [Post("/Identity/Login")]
        Task<UserOutputModel> Login([Body] UserInputModel loginInput);
    }
}
