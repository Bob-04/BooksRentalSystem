using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BooksRentalSystem.Admin.Models.Publishers
{
    public class PublisherFormModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
