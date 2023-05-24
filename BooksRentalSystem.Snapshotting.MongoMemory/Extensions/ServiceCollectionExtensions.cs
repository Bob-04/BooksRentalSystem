﻿using BooksRentalSystem.EventSourcing.Serialization;
using BooksRentalSystem.EventSourcing.Snapshotting;
using BooksRentalSystem.Snapshotting.MongoMemory.SnapshotStores;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Snapshotting.MongoMemory.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongoMemorySnapshotsStore(
        this IServiceCollection serviceCollection,
        string mongoDbConnectionString,
        IEventStoreJsonSerializer jsonSerializer,
        bool allowMemorySnapshots = true
    )
    {
        serviceCollection.AddTransient<ISnapshotStore, MongoMemorySnapshotStore>(_ =>
            new MongoMemorySnapshotStore(mongoDbConnectionString, jsonSerializer, allowMemorySnapshots));

        return serviceCollection;
    }
}