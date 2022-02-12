using AutoMapper;

namespace BooksRentalSystem.Common.Mapping
{
    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<User, UserDto>()
            //    .ForMember(dest => dest.PublishedStatements, opt => opt.MapFrom(u => u.Statements.Count))
            //    .ForMember(dest => dest.MyReviews, opt => opt.MapFrom(u => u.MyReviews.Take(10)));
        }
    }
}
