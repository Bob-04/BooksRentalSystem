using Anabasis.SqlServer.SnapshotStores;
using BooksRentalSystem.EventSourcing.Snapshotting;
using Microsoft.Extensions.DependencyInjection;

namespace Anabasis.SqlServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlSnapshotsStore(
        this IServiceCollection serviceCollection,
        string sqlConnectionString
    )
    {
        serviceCollection.AddTransient<ISnapshotStore, SqlSnapshotStore>(_ =>
            new SqlSnapshotStore(sqlConnectionString));

        return serviceCollection;
    }
}