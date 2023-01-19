using System;
using System.ComponentModel.DataAnnotations;
using BooksRentalSystem.Publishers.Domain.Enums;

namespace BooksRentalSystem.Publishers.Models.BookAds
{
    public class BookAdInputModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }

        [Range(0, int.MaxValue)]
        public decimal PricePerDay { get; set; }

        [Required]
        public string Author { get; set; }

        public int Category { get; set; }
        

        // BookInfo
        public int? PagesNumber { get; set; }

        public string Language { get; set; }

        public DateTime? PublicationDate { get; set; }

        [EnumDataType(typeof(CoverType))]
        public CoverType? Cover { get; set; }
    }
}
