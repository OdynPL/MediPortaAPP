# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Kopiujemy tylko pliki projektów
COPY MediPorta.API/*.csproj ./MediPorta.API/
COPY MediPorta.Application/*.csproj ./MediPorta.Application/
COPY MediPorta.Domain/*.csproj ./MediPorta.Domain/
COPY MediPorta.Infrastructure/*.csproj ./MediPorta.Infrastructure/

# Restore zależności
RUN dotnet restore MediPorta.API/MediPorta.API.csproj

# Kopiujemy resztę kodu
COPY . .

# Publikacja w Release
WORKDIR /app/MediPorta.API
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Kopiujemy opublikowaną aplikację
COPY --from=build /app/publish .

# Ustawienie portu
EXPOSE 5000

# Ustawienie zmiennych środowiskowych dla produkcji
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "MediPorta.API.dll"]
