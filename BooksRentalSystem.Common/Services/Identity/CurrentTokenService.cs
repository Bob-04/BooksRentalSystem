namespace BooksRentalSystem.Common.Services.Identity
{
    public class CurrentTokenService : ICurrentTokenService
    {
        private string _currentToken;

        public string Get() => _currentToken;

        public void Set(string token) => _currentToken = token;
    }
}
