FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/MaisonCalliard.Api/MaisonCalliard.Api.csproj MaisonCalliard.Api/
COPY src/MaisonCalliard.Application/MaisonCalliard.Application.csproj MaisonCalliard.Application/
COPY src/MaisonCalliard.Domain/MaisonCalliard.Domain.csproj MaisonCalliard.Domain/
COPY src/MaisonCalliard.Infrastructure/MaisonCalliard.Infrastructure.csproj MaisonCalliard.Infrastructure/

RUN dotnet restore MaisonCalliard.Api/MaisonCalliard.Api.csproj

COPY src/ .
RUN dotnet publish MaisonCalliard.Api/MaisonCalliard.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MaisonCalliard.Api.dll"]
