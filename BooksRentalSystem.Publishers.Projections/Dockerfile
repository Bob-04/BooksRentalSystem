﻿FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Publishers.Projections/BooksRentalSystem.Publishers.Projections.csproj", "BooksRentalSystem.Publishers.Projections/"]
RUN dotnet restore "BooksRentalSystem.Publishers.Projections/BooksRentalSystem.Publishers.Projections.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Publishers.Projections"
RUN dotnet build "BooksRentalSystem.Publishers.Projections.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Publishers.Projections.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Publishers.Projections.dll"]
