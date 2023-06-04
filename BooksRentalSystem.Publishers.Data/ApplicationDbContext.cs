using System.Reflection;
using BooksRentalSystem.Common.Data;
using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Publishers.Data
{
    public class ApplicationDbContext : MessageDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BookAd> BookAds { get; set; }

        public DbSet<BookInfo> BookInfos { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        protected override Assembly ConfigurationsAssembly => Assembly.GetExecutingAssembly();
    }
}
