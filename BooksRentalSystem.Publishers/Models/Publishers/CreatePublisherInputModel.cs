using System.ComponentModel.DataAnnotations;

namespace BooksRentalSystem.Publishers.Models.Publishers
{
    public class CreatePublisherInputModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MinLength(PublishersConstants.Publishers.MinPhoneNumberLength)]
        [MaxLength(PublishersConstants.Publishers.MaxPhoneNumberLength)]
        [RegularExpression(PublishersConstants.Publishers.PhoneNumberRegularExpression)]
        public string PhoneNumber { get; set; }
    }
}
