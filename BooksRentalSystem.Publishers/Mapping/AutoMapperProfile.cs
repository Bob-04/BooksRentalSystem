using System.Linq;
using AutoMapper;
using BooksRentalSystem.Publishers.Data.Models;
using BooksRentalSystem.Publishers.Models.BookAds;
using BooksRentalSystem.Publishers.Models.Categories;
using BooksRentalSystem.Publishers.Models.Publishers;

namespace BooksRentalSystem.Publishers.Mapping
{
    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Publisher, PublisherOutputModel>();

            CreateMap<Publisher, PublisherDetailsOutputModel>()
                .IncludeBase<Publisher, PublisherOutputModel>()
                .ForMember(p => p.TotalBookAds, cfg => cfg
                    .MapFrom(d => d.BookAds.Count));

            CreateMap<Category, CategoryOutputModel>()
                .ForMember(c => c.TotalBookAds, cfg => cfg
                    .MapFrom(c => c.BookAds.Count(ba => ba.IsAvailable)));

            CreateMap<BookAd, BookAdOutputModel>()
                .ForMember(ad => ad.Author, cfg => cfg
                    .MapFrom(ad => ad.Author.Name))
                .ForMember(ad => ad.Category, cfg => cfg
                    .MapFrom(ad => ad.Category.Name));

            CreateMap<BookAd, MineBookAdOutputModel>()
                .IncludeBase<BookAd, BookAdOutputModel>();

            CreateMap<BookAd, BookAdDetailsOutputModel>()
                .IncludeBase<BookAd, BookAdOutputModel>()
                .ForMember(c => c.PagesNumber, cfg => cfg
                    .MapFrom(c => c.BookInfo.PagesNumber))
                .ForMember(c => c.Language, cfg => cfg
                    .MapFrom(c => c.BookInfo.Language))
                .ForMember(c => c.PublicationDate, cfg => cfg
                    .MapFrom(c => c.BookInfo.PublicationDate))
                .ForMember(c => c.CoverType, cfg => cfg
                    .MapFrom(c => c.BookInfo.CoverType))
                .ForMember(c => c.Publisher, cfg => cfg
                    .MapFrom(c => c.Publisher));
        }
    }
}
