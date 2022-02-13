namespace BooksRentalSystem.Publishers.Data.Models
{
    public class BookAd
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public decimal PricePerDay { get; set; }

        public BookInfo BookInfo { get; set; }

        public string Location { get; set; }

        public bool IsAvailable { get; set; }

        public int PublisherId { get; set; }

        public Publisher Publisher { get; set; }

        public int AuthorId { get; set; }

        public Author Author { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
