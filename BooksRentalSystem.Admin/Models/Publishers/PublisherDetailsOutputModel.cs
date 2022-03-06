namespace BooksRentalSystem.Admin.Models.Publishers
{
    public class PublisherDetailsOutputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public int TotalBookAds { get; private set; }
    }
}
