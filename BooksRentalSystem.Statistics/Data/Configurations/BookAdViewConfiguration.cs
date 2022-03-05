using BooksRentalSystem.Statistics.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksRentalSystem.Statistics.Data.Configurations
{
    public class BookAdViewConfiguration : IEntityTypeConfiguration<BookAdView>
    {
        public void Configure(EntityTypeBuilder<BookAdView> builder)
        {
            builder
                .HasKey(v => v.Id);

            builder
                .HasIndex(v => v.BookAdId);

            builder
                .Property(v => v.UserId)
                .IsRequired();
        }
    }
}
