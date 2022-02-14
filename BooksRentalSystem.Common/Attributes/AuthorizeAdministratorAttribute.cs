using Microsoft.AspNetCore.Authorization;

namespace BooksRentalSystem.Common.Attributes
{
    using static Constants;

    public class AuthorizeAdministratorAttribute : AuthorizeAttribute
    {
        public AuthorizeAdministratorAttribute() => Roles = AdministratorRoleName;
    }
}
