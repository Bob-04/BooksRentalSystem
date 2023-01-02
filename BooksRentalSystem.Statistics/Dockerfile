FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Statistics/BooksRentalSystem.Statistics.csproj", "BooksRentalSystem.Statistics/"]
COPY ["BooksRentalSystem.Common/BooksRentalSystem.Common.csproj", "BooksRentalSystem.Common/"]
RUN dotnet restore "BooksRentalSystem.Statistics/BooksRentalSystem.Statistics.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Statistics"
RUN dotnet build "BooksRentalSystem.Statistics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Statistics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Statistics.dll"]