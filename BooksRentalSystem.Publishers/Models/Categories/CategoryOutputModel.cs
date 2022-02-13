namespace BooksRentalSystem.Publishers.Models.Categories
{
    public class CategoryOutputModel //: IMapFrom<Category>
    {
        public int Id { get; private set; }

        public string Name { get; private set; } = default!;

        public string Description { get; private set; } = default!;

        public int TotalBookAds { get; set; }

        //public void Mapping(Profile profile)
        //    => profile
        //        .CreateMap<Category, CategoryOutputModel>()
        //        .ForMember(c => c.TotalBookAds, cfg => cfg
        //            .MapFrom(c => c.BookAds.Count()));
    }
}
