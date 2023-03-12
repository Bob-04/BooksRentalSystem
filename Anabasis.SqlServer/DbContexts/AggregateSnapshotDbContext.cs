using Anabasis.Common;
using Anabasis.EventStore.Snapshot.SQLServer;
using Microsoft.EntityFrameworkCore;

namespace Anabasis.SqlServer.DbContexts;

public class AggregateSnapshotDbContext : BaseAggregateSnapshotDbContext<AggregateSnapshot>
{
    public AggregateSnapshotDbContext(DbContextOptions options) : base(options)
    {
    }

    public AggregateSnapshotDbContext(DbContextOptionsBuilder dbContextOptionsBuilder) : base(dbContextOptionsBuilder)
    {
    }
}