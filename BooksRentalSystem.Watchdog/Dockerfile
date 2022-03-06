FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Watchdog/BooksRentalSystem.Watchdog.csproj", "BooksRentalSystem.Watchdog/"]
RUN dotnet restore "BooksRentalSystem.Watchdog/BooksRentalSystem.Watchdog.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Watchdog"
RUN dotnet build "BooksRentalSystem.Watchdog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Watchdog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Watchdog.dll"]