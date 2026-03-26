# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar archivos de los proyectos
COPY FantasyWorldCup.Api/*.csproj ./FantasyWorldCup.Api/
COPY FantasyWorldCup.Application/*.csproj ./FantasyWorldCup.Application/
COPY FantasyWorldCup.Domain/*.csproj ./FantasyWorldCup.Domain/
COPY FantasyWorldCup.Infrastructure/*.csproj ./FantasyWorldCup.Infrastructure/

# Restaurar paquetes
RUN dotnet restore FantasyWorldCup.Api/FantasyWorldCup.Api.csproj

# Copiar todo el código
COPY . .

# Publicar el proyecto API
RUN dotnet publish FantasyWorldCup.Api/FantasyWorldCup.Api.csproj -c Release -o /app/publish

# Etapa de producción
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "FantasyWorldCup.Api.dll"]