FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Notifications/BooksRentalSystem.Notifications.csproj", "BooksRentalSystem.Notifications/"]
COPY ["BooksRentalSystem.Common/BooksRentalSystem.Common.csproj", "BooksRentalSystem.Common/"]
RUN dotnet restore "BooksRentalSystem.Notifications/BooksRentalSystem.Notifications.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Notifications"
RUN dotnet build "BooksRentalSystem.Notifications.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Notifications.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Notifications.dll"]