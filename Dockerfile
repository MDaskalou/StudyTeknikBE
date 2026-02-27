FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
# ÄNDRA denna rad till rätt sökväg på din WebAPI .csproj
RUN dotnet publish ./StudyTeknikBE/StudyTeknikBE.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

COPY --from=build /app/publish .
# ÄNDRA DLL-namnet om ditt projekt heter annorlunda
ENTRYPOINT ["dotnet", "StudyTeknikBE.dll"]
