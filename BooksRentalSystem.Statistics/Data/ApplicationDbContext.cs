using System.Reflection;
using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Statistics.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Statistics.Data
{
    public class ApplicationDbContext : MessageDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BookAdView> BookAdViews { get; set; }

        public DbSet<Models.Statistics> Statistics { get; set; }

        public DbSet<ProcessedBookAd> ProcessedBookAds { get; set; }

        protected override Assembly ConfigurationsAssembly => Assembly.GetExecutingAssembly();
    }
}
