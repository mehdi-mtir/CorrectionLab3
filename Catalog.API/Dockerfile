# ===== Stage 1: Base =====
# Image runtime pour .NET 9
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# ===== Stage 2: Build =====
# Image SDK pour compiler le projet
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier les fichiers du projet
COPY ["Catalog.API/Catalog.API.csproj", "Catalog.API/"]
RUN dotnet restore "Catalog.API/Catalog.API.csproj"

# Copier le reste des fichiers et compiler
COPY . .
WORKDIR "/src/Catalog.API"
RUN dotnet build "Catalog.API.csproj" -c Release -o /app/build

# ===== Stage 3: Publish =====
FROM build AS publish
RUN dotnet publish "Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ===== Stage 4: Final =====
# Créer l'image finale légère avec seulement le runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Point d'entrée de l'application
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
