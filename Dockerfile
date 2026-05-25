FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Kopiera projektfiler med EXAKT samma mappstruktur som på datorn
COPY ["src/MaisonCalliard.Api/MaisonCalliard.Api.csproj", "src/MaisonCalliard.Api/"]
COPY ["src/MaisonCalliard.Application/MaisonCalliard.Application.csproj", "src/MaisonCalliard.Application/"]
COPY ["src/MaisonCalliard.Domain/MaisonCalliard.Domain.csproj", "src/MaisonCalliard.Domain/"]
COPY ["src/MaisonCalliard.Infrastructure/MaisonCalliard.Infrastructure.csproj", "src/MaisonCalliard.Infrastructure/"]

# 2. Återställ paket baserat på projektet
RUN dotnet restore "src/MaisonCalliard.Api/MaisonCalliard.Api.csproj"

# 3. Kopiera resten av ALL källkod (Docker tar automatiskt hänsyn till din .dockerignore här)
COPY . .

# 4. Sätt arbetsmappen till API-projektet och bygg
WORKDIR "/src/src/MaisonCalliard.Api"
RUN dotnet publish "MaisonCalliard.Api.csproj" -c Release -o /app/publish --no-restore

# 5. Skapa slutgiltig container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MaisonCalliard.Api.dll"]