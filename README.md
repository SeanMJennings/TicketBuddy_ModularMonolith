# Ticket Buddy
A simple ticket booking platform for events.

## Modular Monolith
Built in well-defined modules to be hosted as a single application. Modules communicate through asynchronous messages using MassTransit with RabbitMQ. 

You could also use synchronous network calls between modules if preferred though it will not scale as well.
In-process calls between modules are monolithic and not recommended.

Strictly speaking, the common libraries are slightly against the modular nature but they help reduce code duplication and improve consistency across modules.

A modular monolith is a good place for a team to start when building a new application.

## Pre-requisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
- [Docker](https://www.docker.com/get-started)
- [Node.js](https://nodejs.org/en/download/)
- [.NET Aspire](https://dotnetaspire.com/)
- Sign in to GitHub and authorise a package feed

## Ways to Run Locally
- A: Ensure docker is running and then run the Host.Aspire project
- B: Ensure docker is running and then run docker compose
- C: Run your own dependencies and manually set appsettings.json files for each project. Then run migrations => API => dataseeder => UI with `npm run dev`

### Pre-seeded Users
There are some initial hardcoded users
- Admin User. Email: admin@ticketbuddy.com Password: admin
- Customer 1. Email: john.smith@example.com Password: johnsmith
- Customer 2. Email: jane.doe@example.com Password: janedoe
- Customer 3. Email: robert.johnson@example.com Password: robertjohnson
- Customer 4. Email: emily.davis@example.com Password: emilydavis


## Architecture Overview
![Modular Monolith Architecture](./Documents/ModularMonolith.drawio.png)

## Architecture Style
The architecture style used is Clean Architecture by [Robert C. Martin (Uncle Bob)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

![Clean Architecture inside Modular Monolith](./Documents/CleanArchitecture.jpg)

## Testing
The testing approach is inspired by the [Modern Test Pyramid](https://dev.to/optivem/modern-test-pyramid-4dfc).

![Modern Test Pyramid](./Documents/ModernTestPyramid.png)

## Key technologies/choices:
- ASP.NET Core
- Docker
- RabbitMQ
- MassTransit
- OpenTelemetry
- .NET Aspire
- Redis Distributed Cache
- PostgreSQL
- Keycloak for user management and auth
- CQRS using Entity Framework Core & Dapper
- React + Vite + Vitest + Playwright for UI

## Observability

Ticket Buddy uses OpenTelemetry to provide comprehensive observability across all services. 
The modular telemetry data is visualized in the Aspire dashboard.

![Observability Architecture](./Documents/Observability.png)
