using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public abstract class BookAdsOutputModel<TBookAdOutputModel>
    {
        protected BookAdsOutputModel(IEnumerable<TBookAdOutputModel> bookAds, int page, int totalBookAds)
        {
            BookAds = bookAds;
            Page = page;
            TotalBookAds = totalBookAds;
        }

        public IEnumerable<TBookAdOutputModel> BookAds { get; }

        public int Page { get; }

        public int TotalBookAds { get; }
    }
}
