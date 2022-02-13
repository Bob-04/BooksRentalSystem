using System;
using BooksRentalSystem.Publishers.Models.Publishers;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class BookAdDetailsOutputModel : BookAdOutputModel
    {
        public int? PagesNumber { get; private set; }

        public string Language { get; private set; }

        public DateTime? PublicationDate { get; private set; }

        public string CoverType { get; private set; }

        public PublisherOutputModel Publisher { get; set; }

        //public override void Mapping(Profile mapper)
        //    => mapper
        //        .CreateMap<CarAd, BookAdDetailsOutputModel>()
        //        .IncludeBase<CarAd, BookAdOutputModel>()
        //        .ForMember(c => c.HasClimateControl, cfg => cfg
        //            .MapFrom(c => c.BookInfo.HasClimateControl))
        //        .ForMember(c => c.NumberOfSeats, cfg => cfg
        //            .MapFrom(c => c.BookInfo.NumberOfSeats))
        //        .ForMember(c => c.CoverType, cfg => cfg
        //            .MapFrom(c => c.BookInfo.CoverType))
        //        .ForMember(c => c.Dealer, cfg => cfg
        //            .MapFrom(c => c.Dealer));
    }
}
