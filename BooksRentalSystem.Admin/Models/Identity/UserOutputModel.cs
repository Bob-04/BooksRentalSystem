namespace BooksRentalSystem.Admin.Models.Identity
{
    public class UserOutputModel
    {
        public UserOutputModel(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
