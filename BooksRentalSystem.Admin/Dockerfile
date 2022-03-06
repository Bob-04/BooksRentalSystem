FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["BooksRentalSystem.Admin/BooksRentalSystem.Admin.csproj", "BooksRentalSystem.Admin/"]
COPY ["BooksRentalSystem.Common/BooksRentalSystem.Common.csproj", "BooksRentalSystem.Common/"]
RUN dotnet restore "BooksRentalSystem.Admin/BooksRentalSystem.Admin.csproj"
COPY . .
WORKDIR "/src/BooksRentalSystem.Admin"
RUN dotnet build "BooksRentalSystem.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BooksRentalSystem.Admin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BooksRentalSystem.Admin.dll"]