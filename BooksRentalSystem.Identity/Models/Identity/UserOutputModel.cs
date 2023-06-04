namespace BooksRentalSystem.Identity.Models.Identity
{
    public class UserOutputModel
    {
        public UserOutputModel(string id, string token)
        {
            Id = id;
            Token = token;
        }

        public string Id { get; }
        public string Token { get; }
    }
}
