FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

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
RUN dotnet restore "LocalHosting/Host.Dataseeder/Host.Dataseeder.csproj"
WORKDIR "/src/LocalHosting/Host.Dataseeder"
RUN dotnet build "Host.Dataseeder.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Host.Dataseeder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Host.Dataseeder.dll"]