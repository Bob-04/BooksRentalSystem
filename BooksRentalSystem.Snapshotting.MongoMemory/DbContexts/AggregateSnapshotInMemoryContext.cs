using BooksRentalSystem.Snapshotting.MongoMemory.DbModels;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Snapshotting.MongoMemory.DbContexts;

public class AggregateSnapshotInMemoryContext : DbContext
{
    public AggregateSnapshotInMemoryContext(DbContextOptions<AggregateSnapshotInMemoryContext> options)
        : base(options)
    {
    }

    public DbSet<AggregateSnapshotSqlModel> AggregateSnapshots { get; set; }
}