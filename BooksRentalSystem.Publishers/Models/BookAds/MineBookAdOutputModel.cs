namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class MineBookAdOutputModel : BookAdOutputModel
    {
        public bool IsAvailable { get; private set; }

        //public override void Mapping(Profile mapper)
        //    => mapper
        //        .CreateMap<BookAd, MineBookAdOutputModel>()
        //        .IncludeBase<BookAd, BookAdOutputModel>();
    }
}
