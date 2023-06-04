using BooksRentalSystem.EventSourcing.Snapshotting;
using BooksRentalSystem.Snapshottings.MongoDb.SnapshotStores;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Snapshottings.MongoDb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDbSnapshotsStore(
        this IServiceCollection serviceCollection,
        string mongoDbConnectionString
    )
    {
        serviceCollection.AddTransient<ISnapshotStore, MongoSnapshotStore>(_ =>
            new MongoSnapshotStore(mongoDbConnectionString));

        return serviceCollection;
    }
}