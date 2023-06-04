using System;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class BookAdOutputModel //: IMapFrom<CarAd>
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; } 

        public string Model { get; set; }

        public string ImageUrl { get; set; }

        public int Category { get; set; } 

        public decimal PricePerDay { get; set; }

        //public virtual void Mapping(Profile mapper)
        //    => mapper
        //        .CreateMap<CarAd, BookAdOutputModel>()
        //        .ForMember(ad => ad.Manufacturer, cfg => cfg
        //            .MapFrom(ad => ad.Manufacturer.Name))
        //        .ForMember(ad => ad.Category, cfg => cfg
        //            .MapFrom(ad => ad.Category.Name));
    }
}
