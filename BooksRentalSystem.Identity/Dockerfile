FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Identity/BooksRentalSystem.Identity.csproj", "BooksRentalSystem.Identity/"]
COPY ["BooksRentalSystem.Common/BooksRentalSystem.Common.csproj", "BooksRentalSystem.Common/"]
RUN dotnet restore "BooksRentalSystem.Identity/BooksRentalSystem.Identity.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Identity"
RUN dotnet build "BooksRentalSystem.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Identity.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Identity.dll"]