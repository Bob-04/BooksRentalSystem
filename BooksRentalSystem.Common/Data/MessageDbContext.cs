using System.Reflection;
using BooksRentalSystem.Common.Data.Configurations;
using BooksRentalSystem.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Common.Data
{
    public abstract class MessageDbContext : DbContext
    {
        protected MessageDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }

        protected abstract Assembly ConfigurationsAssembly { get; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MessageConfiguration());

            builder.ApplyConfigurationsFromAssembly(ConfigurationsAssembly);

            base.OnModelCreating(builder);
        }
    }
}
