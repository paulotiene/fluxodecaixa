FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/Services/Consolidado/Consolidado.Api/Consolidado.Api.csproj
RUN dotnet publish src/Services/Consolidado/Consolidado.Api/Consolidado.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Consolidado.Api.dll"]
