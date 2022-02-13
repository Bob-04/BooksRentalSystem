using System.Collections.Generic;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public abstract class BookAdsOutputModel<TCarAdOutputModel>
    {
        protected BookAdsOutputModel(IEnumerable<TCarAdOutputModel> carAds, int page, int totalCarAds)
        {
            CarAds = carAds;
            Page = page;
            TotalCarAds = totalCarAds;
        }

        public IEnumerable<TCarAdOutputModel> CarAds { get; }

        public int Page { get; }

        public int TotalCarAds { get; }
    }
}
