FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Identity.Projections/BooksRentalSystem.Identity.Projections.csproj", "BooksRentalSystem.Identity.Projections/"]
RUN dotnet restore "BooksRentalSystem.Identity.Projections/BooksRentalSystem.Identity.Projections.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Identity.Projections"
RUN dotnet build "BooksRentalSystem.Identity.Projections.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Identity.Projections.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Identity.Projections.dll"]
