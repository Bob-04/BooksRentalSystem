namespace BooksRentalSystem.Common.Messages.Publishers
{
    public class BookAdUpdatedMessage
    {
        public int BookAdId { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }
    }
}
