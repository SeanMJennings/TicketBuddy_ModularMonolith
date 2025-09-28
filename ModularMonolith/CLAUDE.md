# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TicketBuddy is a modular monolith built with .NET 9.0 following Domain-Driven Design (DDD) and hexagonal architecture principles. The system is organized into three main bounded contexts: **Events**, **Users**, and **Tickets**, with clear module boundaries and integration patterns.

## Architecture

### Modular Monolith Structure
The codebase follows a modular monolith pattern where each module (Events, Users, Tickets) has its own:
- **Domain Layer**: Core business logic and domain models
- **Application Layer**: Use cases and application services
- **Controllers Layer**: HTTP API endpoints and request handlers
- **Infrastructure Layer**: Database access, external services, and technical concerns
- **Integration Layer**: Cross-module messaging and communication

### Key Architectural Patterns
- **Hexagonal Architecture**: Clear separation between business logic and infrastructure
- **CQRS**: Command Query Responsibility Segregation for read/write operations
- **Domain Events**: Loose coupling between modules via event-driven communication
- **BDD Testing**: Behavior-driven development with Testing.Bdd framework

## Development Commands

### Building and Running
```bash
# Build entire solution
dotnet build TicketBuddy.sln

# Run main API (requires Docker services)
dotnet run --project Host

# Run with Aspire (includes all dependencies)
dotnet run --project Host.Aspire

# Docker Compose (full stack)
docker-compose up -d
```

### Database Operations
```bash
# Run migrations
dotnet run --project Host.Migrations

# Seed test data
dotnet run --project Host.Dataseeder
```

### Testing
```bash
# Run all tests
dotnet test TicketBuddy.sln

# Run unit tests for specific module
dotnet test Testing.Unit.Users
dotnet test Testing.Unit.Events

# Run integration tests
dotnet test Testing.Integration

# Run specific test method
dotnet test --filter "method_name"
```

## Module Dependencies

### Cross-Module Communication
Modules communicate via:
- **Domain Events**: Published through MassTransit/RabbitMQ
- **Integration Messaging**: Async messaging between bounded contexts

### Dependency Flow
```
Controllers → Application → Domain
Controllers → Infrastructure (for queries)
Integration.*.Messaging → External modules
```

**Important**: Domain layer should never depend on Application or Infrastructure layers.

## Testing Structure

### BDD Testing Pattern
Tests follow the Given-When-Then pattern using the Testing.Bdd framework:

```csharp
[Test]
public void user_validation_example()
{
    scenario(() =>
    {
        Given(a_user_with_invalid_email);
        When(creating_the_user);
        Then(validation_fails_with_message("Invalid email format"));
    });
}
```

### Test Organization
- **Testing.Unit.{Module}**: Unit tests for domain logic
- **Testing.Integration**: Full stack integration tests with TestContainers
- **{Module}.specs.cs**: BDD test scenarios
- **{Module}.steps.cs**: Test step implementations

## Key Technologies

- **.NET 9.0**: Target framework
- **PostgreSQL**: Primary database (via Docker)
- **MassTransit + RabbitMQ**: Message bus for domain events
- **Redis**: Caching layer
- **Aspire**: Local development orchestration
- **TestContainers**: Integration testing with real databases
- **OpenTelemetry**: Observability and tracing

## Development Guidelines

### Domain-Driven Design
Follow DDD principles as outlined in `.claude/ddd-summary.md`:
- Rich domain models with business logic encapsulated in entities
- Aggregates maintain consistency boundaries
- Domain events for cross-aggregate communication
- Ubiquitous language reflected in code

### Code Organization
```
Domain.{Module}/          # Core business logic
├── Entities/            # Domain entities and aggregates
├── Events/              # Domain events
├── Repositories/        # Repository interfaces
└── Services/            # Domain services

Application.{Module}/     # Application services and use cases
├── Commands/            # Command handlers (write operations)
├── Queries/             # Query handlers (read operations)
└── EventHandlers/       # Domain event handlers

Controllers.{Module}/     # HTTP API endpoints
Infrastructure.{Module}/  # Data access and external services
Integration.{Module}.Messaging/  # Cross-module messaging
```

### Testing Best Practices
- Use BDD-style tests for domain behavior
- Integration tests with TestContainers for database scenarios
- Separate test specifications from step implementations
- Follow naming convention: `method_does_something_when_condition`

## Local Development Setup

### Prerequisites
- .NET 9.0 SDK
- Docker Desktop
- Git

### First-Time Setup
1. Clone repository
2. Start infrastructure: `docker-compose up -d postgresql rabbitmq redis`
3. Run migrations: `dotnet run --project Host.Migrations`
4. Seed data: `dotnet run --project Host.Dataseeder`
5. Start API: `dotnet run --project Host` or use Aspire: `dotnet run --project Host.Aspire`

### Development Workflow
- API runs on `http://localhost:5000` (Docker) or configured Aspire ports
- Aspire Dashboard: `http://localhost:18888` (when using Aspire)
- RabbitMQ Management: `http://localhost:15672` (guest/guest)
- PostgreSQL: `localhost:5432` (postgres/YourStrong@Passw0rd)

## Important Notes

- **Module Boundaries**: Respect module boundaries - don't directly reference domain objects across modules
- **Domain Purity**: Keep domain layer free of infrastructure concerns
- **Event-Driven Integration**: Use domain events for cross-module communication
- **BDD Testing**: Write tests that express business behavior, not implementation details
- **Comprehensive Documentation**: Extensive architectural guidance available in `.claude/` directory