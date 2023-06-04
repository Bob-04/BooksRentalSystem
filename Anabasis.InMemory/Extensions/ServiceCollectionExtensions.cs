using Anabasis.InMemory.SnapshotStores;
using BooksRentalSystem.EventSourcing.Snapshotting;
using Microsoft.Extensions.DependencyInjection;

namespace Anabasis.InMemory.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemorySnapshotsStore(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ISnapshotStore, InMemorySnapshotStore>();

        return serviceCollection;
    }
}