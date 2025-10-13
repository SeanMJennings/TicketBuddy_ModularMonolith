FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Modules/ Modules/
COPY CommonLibraries/ CommonLibraries/
COPY Host/ Host/
COPY Database/ Database/
COPY . .
RUN dotnet restore "Host/Host.csproj"
WORKDIR "/src/Host"
RUN dotnet build "Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Host"
RUN dotnet publish "Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Does not seem to work?
RUN apt-get update && apt-get --yes install curl

ENTRYPOINT ["dotnet", "Host.dll"]