using System.Collections.Generic;
using System.Threading.Tasks;
using BooksRentalSystem.Admin.Models.Publishers;
using Refit;

namespace BooksRentalSystem.Admin.Services.Publishers
{
    public interface IPublishersService
    {
        [Get("/Publishers")]
        Task<IEnumerable<PublisherDetailsOutputModel>> All();

        [Get("/Publishers/{id}")]
        Task<PublisherDetailsOutputModel> Details(int id);

        [Put("/Publishers/{id}")]
        Task Edit(int id, PublisherInputModel publisher);
    }
}
