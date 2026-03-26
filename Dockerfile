# Usar la imagen oficial de .NET 8 SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar todos los proyectos y restaurar paquetes
COPY . ./
RUN dotnet restore

# Publicar el proyecto LibrarySearch.Web en Release
RUN dotnet publish LibrarySearch.Web -c Release -o /app/out

# Imagen final para correr la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Comando para ejecutar la app
ENTRYPOINT ["dotnet", "LibrarySearch.Web.dll"]
