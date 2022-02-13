namespace BooksRentalSystem.Publishers.Models.Publishers
{
    public class PublisherDetailsOutputModel : PublisherOutputModel
    {
        public int TotalBookAds { get; private set; }

        //public void Mapping(Profile mapper)
        //    => mapper
        //        .CreateMap<Dealer, AuthorDetailsOutputModel>()
        //        .IncludeBase<Dealer, AuthorOutputModel>()
        //        .ForMember(d => d.TotalBookAds, cfg => cfg
        //            .MapFrom(d => d.BookAds.Count()));
    }
}
