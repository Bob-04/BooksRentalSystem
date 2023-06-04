using System;
using System.ComponentModel.DataAnnotations;

namespace BooksRentalSystem.Statistics.Data.Models;

public class ProcessedBookAd
{
    [Key] public Guid Id { get; set; }
}