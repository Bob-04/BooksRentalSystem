using System.Linq;
using System.Threading.Tasks;
using BooksRentalSystem.Common;
using BooksRentalSystem.Common.Services.Data;
using BooksRentalSystem.Common.Settings;
using BooksRentalSystem.Identity.Data.Models;
using BooksRentalSystem.Identity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Constants = BooksRentalSystem.Common.Constants;

namespace BooksRentalSystem.Identity.Data.Seeding
{
    public class IdentityDataSeeder : IDataSeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationSettings _applicationSettings;
        private readonly IdentitySettings _identitySettings;

        public IdentityDataSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<ApplicationSettings> applicationSettings, IOptions<IdentitySettings> identitySettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationSettings = applicationSettings.Value;
            _identitySettings = identitySettings.Value;
        }

        public void SeedData()
        {
            if (!_roleManager.Roles.Any())
            {
                Task.Run(async () =>
                    {
                        var adminRole = new IdentityRole(Constants.AdministratorRoleName);

                        await _roleManager.CreateAsync(adminRole);

                        var adminUser = new User
                        {
                            UserName = "admin",
                            Email = "admin@books.com",
                            SecurityStamp = "RandomSecurityStamp"
                        };

                        await _userManager.CreateAsync(adminUser, _identitySettings.AdminPassword);

                        await _userManager.AddToRoleAsync(adminUser, Constants.AdministratorRoleName);
                    })
                    .GetAwaiter()
                    .GetResult();
            }

            if (_applicationSettings.SeedInitialData)
            {
                Task.Run(async () =>
                    {
                        if (await _userManager.FindByIdAsync(DataSeederConstants.DefaultUserId) != null)
                        {
                            return;
                        }

                        var defaultUser = new User
                        {
                            Id = DataSeederConstants.DefaultUserId,
                            UserName = "coolbooks",
                            Email = "coolbooks@books.com"
                        };

                        await _userManager.CreateAsync(defaultUser, DataSeederConstants.DefaultUserPassword);
                    })
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
