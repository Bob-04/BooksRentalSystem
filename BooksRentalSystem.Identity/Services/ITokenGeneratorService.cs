using System.Collections.Generic;
using BooksRentalSystem.Identity.Data.Models;

namespace BooksRentalSystem.Identity.Services
{
    public interface ITokenGeneratorService
    {
        string GenerateToken(User user, IEnumerable<string> roles = null);
    }
}
