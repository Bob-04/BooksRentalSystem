using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<BookAd> BookAds { get; set; } = new List<BookAd>();
    }
}
