FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG TICKETBUDDY_GITHUB_TOKEN
WORKDIR /src
COPY . .
RUN dotnet nuget update source TicketBuddyGitHub \
    --username SeanMJennings \
    --password $TICKETBUDDY_GITHUB_TOKEN \
    --store-password-in-clear-text
RUN dotnet restore "LocalHosting/LocalHost.Dataseeder/LocalHost.Dataseeder.csproj"
WORKDIR "/src/LocalHosting/LocalHost.Dataseeder"
RUN dotnet build "LocalHost.Dataseeder.csproj" -c $BUILD_CONFIGURATION --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LocalHost.Dataseeder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "LocalHost.Dataseeder.dll"]