using System;
using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Data.Models
{
    public class Author
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? BirthDate { get; set; }

        public IEnumerable<BookAd> CarAds { get; set; } = new List<BookAd>();
    }
}
