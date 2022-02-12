using System.Security.Claims;

namespace BooksRentalSystem.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdministrator(this ClaimsPrincipal user)
            => user.IsInRole(Constants.AdministratorRoleName);
    }
}
