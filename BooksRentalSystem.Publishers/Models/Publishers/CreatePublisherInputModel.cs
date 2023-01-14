using System.ComponentModel.DataAnnotations;
using BooksRentalSystem.Common;

namespace BooksRentalSystem.Publishers.Models.Publishers
{
    public class CreatePublisherInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(UserConstants.MinPhoneNumberLength)]
        [MaxLength(UserConstants.MaxPhoneNumberLength)]
        [RegularExpression(UserConstants.PhoneNumberRegularExpression)]
        public string PhoneNumber { get; set; }
    }
}
