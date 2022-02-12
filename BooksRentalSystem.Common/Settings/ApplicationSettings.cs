namespace BooksRentalSystem.Common.Settings
{
    public class ApplicationSettings
    {
        public string Secret { get; private set; }

        public bool SeedInitialData { get; private set; }
    }
}
