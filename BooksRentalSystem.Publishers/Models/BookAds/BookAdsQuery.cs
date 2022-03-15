namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class BookAdsQuery
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public int? Category { get; set; }

        public decimal? MinPricePerDay { get; set; }

        public decimal? MaxPricePerDay { get; set; }

        public string SortBy { get; set; }

        public string Order { get; set; }

        public int Page { get; set; } = 1;
    }
}
