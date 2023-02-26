using System;

namespace BooksRentalSystem.Common.Messages.Publishers
{
    public class BookAdCreatedMessage
    {
        public Guid BookAdId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public decimal PricePerDay { get; set; }
    }
}
