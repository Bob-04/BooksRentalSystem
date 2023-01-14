using System;

namespace BooksRentalSystem.Publishers.Data.Models
{
    public class BookInfo
    {
        public int? PagesNumber { get; set; }

        public string Language { get; set; }

        public DateTime? PublicationDate { get; set; }

        public CoverType? CoverType { get; set; }
    }
}
