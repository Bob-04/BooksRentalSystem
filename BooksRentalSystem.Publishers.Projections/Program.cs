using BooksRentalSystem.Common.Extensions;
using BooksRentalSystem.EventSourcing.Extensions;
using BooksRentalSystem.Publishers.Data;
using BooksRentalSystem.Publishers.Projections.HostedServices;
using BooksRentalSystem.Publishers.Projections.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebService<ApplicationDbContext>(builder.Configuration, messagingHealthChecks: false);

builder.Services
    .AddTransient<IPublishersService, PublishersService>()
    .AddTransient<ICategoryService, CategoryService>()
    .AddTransient<IBookAdsService, BookAdsService>()
    .AddTransient<IAuthorsService, AuthorsService>();

var eventStoreCs = builder.Configuration.GetConnectionString("EventStore") ??
                   throw new InvalidOperationException();

builder.Services
    .AddEventStoreSubscription<UserHostedService>(eventStoreCs)
    .AddEventStoreSubscription<BookAdsHostedService>(eventStoreCs);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();