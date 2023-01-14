using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksRentalSystem.Publishers.Data.Configurations
{
    internal class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
    {
        public void Configure(EntityTypeBuilder<Publisher> builder)
        {
            builder
                .HasKey(p => p.Id);

            builder
                .Property(p => p.Name)
                .IsRequired();

            builder
                .Property(p => p.PhoneNumber)
                .HasMaxLength(PublishersConstants.Publishers.MaxPhoneNumberLength);

            builder
                .Property(p => p.UserId)
                .IsRequired();

            builder
                .HasMany(p => p.BookAds)
                .WithOne(b => b.Publisher)
                .HasForeignKey(b => b.PublisherId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
