using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksRentalSystem.Publishers.Data.Configurations
{
    internal class BookAdConfiguration : IEntityTypeConfiguration<BookAd>
    {
        public void Configure(EntityTypeBuilder<BookAd> builder)
        {
            builder
                .HasKey(b => b.Id);

            builder
                .Property(b => b.Id)
                .UseHiLo(nameof(BookAd));

            builder
                .Property(b => b.PricePerDay)
                .IsRequired();

            builder
                .Property(b => b.IsAvailable)
                .IsRequired();

            builder
                .HasOne(b => b.Author)
                .WithMany(a => a.BookAds)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.Category)
                .WithMany(c => c.BookAds)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .OwnsOne(b => b.BookInfo, options =>
                {
                    options.WithOwner();
                });
        }
    }
}
