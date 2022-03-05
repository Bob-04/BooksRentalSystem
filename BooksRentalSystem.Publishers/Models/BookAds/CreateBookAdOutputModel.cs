namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class CreateBookAdOutputModel
    {
        public CreateBookAdOutputModel(int bookAdId)
        {
            BookAdId = bookAdId;
        }

        public int BookAdId { get; }
    }
}
