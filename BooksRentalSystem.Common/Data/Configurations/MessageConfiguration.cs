using System;
using BooksRentalSystem.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BooksRentalSystem.Common.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property<string>("serializedData")
                .IsRequired()
                .HasField("serializedData");

            builder
                .Property(m => m.Type)
                .IsRequired()
                .HasConversion(
                    t => t.AssemblyQualifiedName,
                    t => Type.GetType(t));
        }
    }
}
