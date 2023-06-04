using System.ComponentModel.DataAnnotations;
using BooksRentalSystem.Common;

namespace BooksRentalSystem.Identity.Models.Identity
{
    public class UserInputModel
    {
        [Required]
        [EmailAddress]
        [MinLength(3)]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required] public string Name { get; set; }

        [Required]
        [MinLength(UserConstants.MinPhoneNumberLength)]
        [MaxLength(UserConstants.MaxPhoneNumberLength)]
        [RegularExpression(UserConstants.PhoneNumberRegularExpression)]
        public string PhoneNumber { get; set; }

        [Required] public string Password { get; set; }
    }
}
