using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Data.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string UserId { get; set; }

        public ICollection<BookAd> BookAds { get; set; } = new List<BookAd>();
    }
}