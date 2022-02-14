using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksRentalSystem.Publishers.Data.Configurations
{
    internal class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder
                .HasKey(a => a.Id);

            builder
                .Property(a => a.Name)
                .IsRequired();
        }
    }
}
