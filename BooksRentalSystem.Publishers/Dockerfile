FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Publishers/BooksRentalSystem.Publishers.csproj", "BooksRentalSystem.Publishers/"]
RUN dotnet restore "BooksRentalSystem.Publishers/BooksRentalSystem.Publishers.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Publishers"
RUN dotnet build "BooksRentalSystem.Publishers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Publishers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Publishers.dll"]