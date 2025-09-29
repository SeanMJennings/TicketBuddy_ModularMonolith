FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Modules/Events/Domain.Events/Domain.Events.csproj", "Domain.Events/"]
COPY ["Modules/Tickets/Domain.Tickets/Domain.Tickets.csproj", "Domain.Tickets/"]
COPY ["Modules/Users/Domain.Users/Domain.Users.csproj", "Domain.Users/"]
COPY ["CommonLibraries/Domain/Domain.csproj", "Domain/"]
COPY ["Modules/Events/Application.Events/Application.Events.csproj", "Application.Events/"]
COPY ["Modules/Tickets/Application.Tickets/Application.Tickets.csproj", "Application.Tickets/"]
COPY ["Modules/Users/Application.Users/Application.Users.csproj", "Application.Users/"]
COPY ["CommonLibraries/Application/Application.csproj", "Application/"]
COPY ["Modules/Events/Controllers.Events/Controllers.Events.csproj", "Controllers.Events/"]
COPY ["Modules/Tickets/Controllers.Tickets/Controllers.Tickets.csproj", "Controllers.Tickets/"]
COPY ["Modules/Users/Controllers.Users/Controllers.Users.csproj", "Controllers.Users/"]
COPY ["Modules/Events/Integration.Events.Messaging/Integration.Events.Messaging.csproj", "Integration.Events.Messaging/"]
COPY ["Modules/Tickets/Integration.Tickets.Messaging/Integration.Tickets.Messaging.csproj", "Integration.Tickets.Messaging/"]
COPY ["Modules/Users/Integration.Users.Messaging/Integration.Users.Messaging.csproj", "Integration.Users.Messaging/"]
COPY ["CommonLibraries/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Modules/Events/Infrastructure.Events/Infrastructure.Events.csproj", "Infrastructure.Events/"]
COPY ["Modules/Users/Infrastructure.Users/Infrastructure.Users.csproj", "Infrastructure.Users/"]
COPY ["Modules/Tickets/Infrastructure.Tickets/Infrastructure.Tickets.csproj", "Infrastructure.Tickets/"]
COPY ["Host/Host.csproj", "Host/"]
RUN dotnet restore "Host/Host.csproj"
COPY . .
WORKDIR "/src/Host"
RUN dotnet build "Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Host"
RUN dotnet publish "Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Host.dll"]