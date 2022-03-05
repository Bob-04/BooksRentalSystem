using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class SearchBookAdsOutputModel : BookAdsOutputModel<BookAdOutputModel>
    {
        public SearchBookAdsOutputModel(IEnumerable<BookAdOutputModel> bookAds, int page, int totalBookAds)
            : base(bookAds, page, totalBookAds)
        {
        }
    }
}
