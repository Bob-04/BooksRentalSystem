using System;
using System.ComponentModel.DataAnnotations;
using BooksRentalSystem.Common;
using BooksRentalSystem.Publishers.Data.Models;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class BookAdInputModel
    {
        [Required]
        [MinLength(DataConstants.MinNameLength)]
        [MaxLength(DataConstants.MaxNameLength)]
        public string Author { get; set; }

        [Required]
        [MinLength(PublishersConstants.CarAds.MinModelLength)]
        [MaxLength(PublishersConstants.CarAds.MaxModelLength)]
        public string Model { get; set; }

        public int Category { get; set; }

        [Required]
        [Url]
        public string ImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public decimal PricePerDay { get; set; }

        public int? PagesNumber { get; set; }

        public string Language { get; set; }

        public DateTime? PublicationDate { get; set; }

        [EnumDataType(typeof(CoverType))]
        public CoverType Cover { get; set; }
    }
}
