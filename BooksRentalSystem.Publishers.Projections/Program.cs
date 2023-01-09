using BooksRentalSystem.EventSourcing.Extensions;
using BooksRentalSystem.Publishers.Projections.HostedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEventStoreSubscription<UserHostedService>("esdb://localhost:2115?tls=false");

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();