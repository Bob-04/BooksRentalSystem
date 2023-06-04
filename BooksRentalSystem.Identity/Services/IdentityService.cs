using System;
using System.Linq;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Models;
using BooksRentalSystem.EventSourcing.Repositories;
using BooksRentalSystem.Identity.Data.Models;
using BooksRentalSystem.Identity.Domain;
using BooksRentalSystem.Identity.Models.Identity;
using Microsoft.AspNetCore.Identity;

namespace BooksRentalSystem.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenGeneratorService _jwtTokenGenerator;
        private readonly IEventStoreAggregateRepository _eventStoreAggregateRepository;

        public IdentityService(UserManager<User> userManager, ITokenGeneratorService jwtTokenGenerator,
            IEventStoreAggregateRepository eventStoreAggregateRepository)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _eventStoreAggregateRepository = eventStoreAggregateRepository;
        }

        public async Task<Result<User>> Register(UserInputModel userInput)
        {
            var user = new User
            {
                Email = userInput.Email,
                UserName = userInput.Email.Split('@')[0]
            };

            var identityResult = await _userManager.CreateAsync(user, userInput.Password);
            if (identityResult.Succeeded)
            {
                var userId = Guid.Parse(user.Id);

                var userAggregate = new UserAggregate { Id = userId };
                userAggregate.CreateUser(userId, userInput.Name, user.Email, userInput.PhoneNumber);
                await _eventStoreAggregateRepository.SaveAsync(userAggregate);
            }

            var errors = identityResult.Errors.Select(e => e.Description);
            
            return identityResult.Succeeded
                ? Result<User>.SuccessWith(user)
                : Result<User>.Failure(errors);
        }

        public async Task<Result<UserOutputModel>> Login(UserLoginModel userInput)
        {
            var user = await _userManager.FindByEmailAsync(userInput.Email);
            if (user == null)
                return "Invalid credentials.";

            var passwordValid = await _userManager.CheckPasswordAsync(user, userInput.Password);
            if (!passwordValid)
                return "Invalid credentials.";

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return new UserOutputModel(user.Id, token);
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

        public async Task<Result> EditUser(Guid userId, EditUserInputModel editUserInput)
        {
            var userAggregate = await _eventStoreAggregateRepository.LoadAsync<UserAggregate>(userId);
            if (userAggregate.Id == default)
                return "Not found";

            userAggregate.UpdateUser(userId, editUserInput.Name, editUserInput.PhoneNumber);
            await _eventStoreAggregateRepository.SaveAsync(userAggregate);

            return Result.Success;
        }
    }
}
