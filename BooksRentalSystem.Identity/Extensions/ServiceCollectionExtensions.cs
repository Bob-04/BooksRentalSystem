using BooksRentalSystem.Identity.Data;
using BooksRentalSystem.Identity.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUsersStorage(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddIdentity<User, IdentityRole>(options =>
                {
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                        "абвгґдеєёжзиіїйклмнопрстуфхцчшщъыьэюяАБВГҐДЕЄЁЖЗИІЇЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ" +
                        "0123456789-._";
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            return serviceCollection;
        }
    }
}
