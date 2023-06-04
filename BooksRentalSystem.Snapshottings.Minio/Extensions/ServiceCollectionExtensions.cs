using BooksRentalSystem.EventSourcing.Snapshotting;
using BooksRentalSystem.Snapshottings.Minio.Options;
using BooksRentalSystem.Snapshottings.Minio.SnapshotStores;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace BooksRentalSystem.Snapshottings.Minio.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlSnapshotsStore(
        this IServiceCollection serviceCollection,
        MinioOptions options
    )
    {
        serviceCollection.AddTransient<ISnapshotStore, MinioSnapshotStore>(_ =>
            new MinioSnapshotStore(
                new MinioClient()
                    .WithEndpoint(options.Endpoint)
                    .WithCredentials(options.AccessKey, options.SecretKey)
                    .Build()
            ));

        return serviceCollection;
    }
}