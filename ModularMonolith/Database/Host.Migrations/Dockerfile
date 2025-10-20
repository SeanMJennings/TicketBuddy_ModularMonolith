FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG GITHUB_TOKEN
WORKDIR /src
COPY . .
RUN dotnet nuget add source https://nuget.pkg.github.com/SeanMJennings/index.json \
    --name github \
    --username SeanMJennings \
    --password $GITHUB_TOKEN \
    --store-password-in-clear-text
RUN dotnet restore "Database/Host.Migrations/Host.Migrations.csproj"
WORKDIR "/src/Database/Host.Migrations"
RUN dotnet build "Host.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Host.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.Migrations.dll"]
