using System.Linq;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Models;
using BooksRentalSystem.Identity.Data.Models;
using BooksRentalSystem.Identity.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace BooksRentalSystem.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenGeneratorService _jwtTokenGenerator;

        public IdentityService(UserManager<User> userManager, ITokenGeneratorService jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<Result<User>> Register(UserInputModel userInput)
        {
            var user = new User
            {
                Email = userInput.Email,
                UserName = userInput.Email
            };

            var identityResult = await _userManager.CreateAsync(user, userInput.Password);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result<User>.SuccessWith(user)
                : Result<User>.Failure(errors);
        }

        public async Task<Result<UserOutputModel>> Login(UserInputModel userInput)
        {
            var user = await _userManager.FindByEmailAsync(userInput.Email);
            if (user == null)
            {
                return "Invalid credentials.";
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, userInput.Password);
            if (!passwordValid)
            {
                return "Invalid credentials.";
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return new UserOutputModel(token);
        }

        public async Task<Result> ChangePassword(string userId, ChangePasswordInputModel changePasswordInput)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return "Invalid credentials.";
            }

            var identityResult = await _userManager.ChangePasswordAsync(user, changePasswordInput.CurrentPassword,
                changePasswordInput.NewPassword);

            var errors = identityResult.Errors.Select(e => e.Description);

            return identityResult.Succeeded
                ? Result.Success
                : Result.Failure(errors);
        }
    }
}
