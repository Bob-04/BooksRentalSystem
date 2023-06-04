using System;
using System.Threading;
using System.Threading.Tasks;
using BooksRentalSystem.EventSourcing.EventHandlers;
using BooksRentalSystem.Publishers.Domain.Events;
using BooksRentalSystem.Statistics.Data;
using BooksRentalSystem.Statistics.Data.Models;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BooksRentalSystem.Statistics.EventHandlers;

public class BookAdCreatedEventHandler : EventStoreEventHandler<BookAdCreatedEvent, object>
{
    private readonly IServiceProvider _serviceProvider;

    public BookAdCreatedEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task<(object, AfterSavingActionStore)> HandleAsync(
        BookAdCreatedEvent @event,
        ResolvedEvent resolvedEvent,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        using var serviceScope = _serviceProvider.CreateScope();

        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var isDuplicated = await dbContext.ProcessedBookAds
            .AnyAsync(ba => ba.Id == @event.Id, cancellationToken);

        if (!isDuplicated)
        {
            var statistics = await dbContext.Statistics.SingleOrDefaultAsync(cancellationToken);
            statistics.TotalBookAds++;

            dbContext.Add(new ProcessedBookAd { Id = @event.Id });

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return default;
    }

    public override async Task AfterActionApplierAsync(
        BookAdCreatedEvent @event,
        object documentToUpdate,
        AfterSavingActionStore afterSaveActions,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Task.CompletedTask;
    }
}