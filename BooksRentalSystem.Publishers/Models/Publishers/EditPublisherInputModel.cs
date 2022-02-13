using System.ComponentModel.DataAnnotations;
using BooksRentalSystem.Common;

namespace BooksRentalSystem.Publishers.Models.Publishers
{
    public class EditPublisherInputModel
    {
        [Required]
        [MinLength(DataConstants.MinNameLength)]
        [MaxLength(DataConstants.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(PublishersConstants.Publishers.MinPhoneNumberLength)]
        [MaxLength(PublishersConstants.Publishers.MaxPhoneNumberLength)]
        [RegularExpression(PublishersConstants.Publishers.PhoneNumberRegularExpression)]
        public string PhoneNumber { get; set; }
    }
}
