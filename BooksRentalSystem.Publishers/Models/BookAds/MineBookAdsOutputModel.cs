using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class MineBookAdsOutputModel : BookAdsOutputModel<MineBookAdOutputModel>
    {
        public MineBookAdsOutputModel(IEnumerable<MineBookAdOutputModel> bookAds, int page, int totalBookAds)
            : base(bookAds, page, totalBookAds)
        {
        }
    }
}
