# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 💡 CAMBIO CLAVE: Utilizamos '..' para subir al directorio raíz del proyecto 
#    donde se encuentra GestionSubscripciones.csproj
COPY ["GestionSubscripciones.csproj", "."] 
RUN dotnet restore "GestionSubscripciones.csproj"

# Copiar el resto del código (incluyendo la carpeta api/ si hay más archivos)
# Si todos tus controllers, models, etc. están en la raíz, solo copia la raíz:
COPY . . 

# Publicar la aplicación para producción
RUN dotnet publish "GestionSubscripciones.csproj" -c Release -o /app/publish

# Stage 2: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GestionSubscripciones.dll"]