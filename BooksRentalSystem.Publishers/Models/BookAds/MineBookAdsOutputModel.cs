using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class MineBookAdsOutputModel : BookAdsOutputModel<MineBookAdOutputModel>
    {
        public MineBookAdsOutputModel(IEnumerable<MineBookAdOutputModel> carAds, int page, int totalCarAds)
            : base(carAds, page, totalCarAds)
        {
        }
    }
}
