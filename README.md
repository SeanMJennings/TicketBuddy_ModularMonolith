# Ticket Buddy
A simple ticket booking platform for events.

Built as both a modular monolith and as microservices to compare and contrast.

## Modular Monolith
Built in well-defined modules to be hosted as a single application. Modules communicate through asynchronous messages using MassTransit with RabbitMQ. 

You could also use synchronous network calls between modules if preferred though it will not scale as well.
In-process calls between modules are monolithic and not recommended.

A modular monolith is where a team should start when building a new application.

[Modular Monolith Architecture](./ModularMonolith/README.md)

## Microservices
Built with a distributed architecture designed to run locally. Services communicate through asynchronous messages using MassTransit with RabbitMQ.
The aim is to demonstrate a fully observable and decoupled system.
Admittedly, the transactional outbox pattern is overkill.

Microservices are a good choice when the application is expected to grow significantly, or when different teams will work on different parts of the application.

These microservices are in one repo and one dotnet solution, but they can be split into multiple repositories and solutions if needed and referenced as NuGet packages.

[Microservices Architecture](./Microservices/README.md)

## Key technologies/choices:
- ASP.NET Core
- Docker
- RabbitMQ
- MassTransit
- OpenTelemetry
- .NET Aspire
- Redis Distributed Cache
- YARP (Microservices)
- Transactional Outbox Pattern (Microservices)
- React + Vite + Vitest

## Observability

Ticket Buddy uses OpenTelemetry to provide comprehensive observability across all services. 
The monolithic and distributed telemetry data are visualized in the Aspire dashboard.

![Observability Architecture](./Observability.png)
