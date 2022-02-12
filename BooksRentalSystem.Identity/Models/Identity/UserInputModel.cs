using System.ComponentModel.DataAnnotations;

namespace BooksRentalSystem.Identity.Models.Identity
{
    public class UserInputModel
    {
        [Required]
        [EmailAddress]
        [MinLength(3)]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
