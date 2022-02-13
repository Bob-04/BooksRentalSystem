namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class CreateBookAdOutputModel
    {
        public CreateBookAdOutputModel(int carAdId)
        {
            CarAdId = carAdId;
        }

        public int CarAdId { get; }
    }
}
