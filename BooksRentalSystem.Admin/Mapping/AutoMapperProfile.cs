using AutoMapper;
using BooksRentalSystem.Admin.Models.Identity;
using BooksRentalSystem.Admin.Models.Publishers;

namespace BooksRentalSystem.Admin.Mapping
{
    internal class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LoginFormModel, UserInputModel>();

            CreateMap<PublisherDetailsOutputModel, PublisherFormModel>();

            CreateMap<PublisherFormModel, PublisherInputModel>();
        }
    }
}
