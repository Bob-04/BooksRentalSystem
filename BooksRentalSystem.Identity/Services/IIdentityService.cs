using System.Threading.Tasks;
using BooksRentalSystem.Common.Models;
using BooksRentalSystem.Identity.Data.Models;
using BooksRentalSystem.Identity.Models.Identity;

namespace BooksRentalSystem.Identity.Services
{
    public interface IIdentityService
    {
        Task<Result<User>> Register(UserInputModel userInput);
        Task<Result<UserOutputModel>> Login(UserLoginModel userInput);
        Task<Result> ChangePassword(string userId, ChangePasswordInputModel changePasswordInput);
    }
}
