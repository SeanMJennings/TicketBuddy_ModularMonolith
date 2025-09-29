FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Host.Migrations/Host.Migrations.csproj", "Host.Migrations/"]
COPY ["Migrations/Migrations.csproj", "Migrations/"]
RUN dotnet restore "Host.Migrations/Host.Migrations.csproj"
COPY . .
WORKDIR "/src/Host.Migrations"
RUN dotnet build "Host.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Host.Migrations.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Host.Migrations.dll"]
