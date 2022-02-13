using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class SearchBookAdsOutputModel : BookAdsOutputModel<BookAdOutputModel>
    {
        public SearchBookAdsOutputModel(IEnumerable<BookAdOutputModel> carAds, int page, int totalCarAds)
            : base(carAds, page, totalCarAds)
        {
        }
    }
}
