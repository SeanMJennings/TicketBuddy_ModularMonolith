FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
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
RUN dotnet restore "Host/Host.csproj"
RUN dotnet build "Host/Host.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src"
RUN dotnet publish "Host/Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Does not seem to work?
RUN apt-get update && apt-get --yes install curl

ENTRYPOINT ["dotnet", "Host.dll"]