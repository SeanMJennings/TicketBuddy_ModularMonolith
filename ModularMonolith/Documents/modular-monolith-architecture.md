# TicketBuddy Modular Monolith - Architecture Guide

## Purpose

TicketBuddy is an event ticketing platform built as a modular monolith. This architecture balances the organizational benefits of microservices (module isolation, clear boundaries, independent testing) with the operational simplicity of a monolith (single deployment, shared infrastructure, simplified development).

**Problem it solves:** Provides a scalable, maintainable event ticketing platform without the operational overhead of distributed systems.

**Why modular monolith:** Allows independent module development and clear bounded contexts while keeping deployment and infrastructure simple during early development.

---

## Table of Contents

- [System Overview](#system-overview)
- [Module Structure](#module-structure)
- [Layering Within Modules](#layering-within-modules)
- [Common Libraries](#common-libraries)
- [Technology Stack](#technology-stack)
- [Module Communication Patterns](#module-communication-patterns)
- [Testing Strategy](#testing-strategy)
- [Development Workflow](#development-workflow)
- [Architectural Decisions](#architectural-decisions)

---

## System Overview

### High-Level Architecture

The system consists of:
- **Single ASP.NET Core Host** - Single deployment unit hosting all modules
- **3 Business Modules** - Events, Tickets, Keycloak.Users
- **Shared Infrastructure** - PostgreSQL, Redis, RabbitMQ, Keycloak
- **Common Libraries** - Reusable domain patterns and infrastructure

```
┌─────────────────────────────────────────────────────────────┐
│                    TicketBuddy REST API                      │
│                   (Single ASP.NET Host)                      │
├─────────────────────────────────────────────────────────────┤
│  Events Module  │  Tickets Module  │  Keycloak.Users Module │
├─────────────────────────────────────────────────────────────┤
│              Common Libraries (Domain, App, Infra)           │
├─────────────────────────────────────────────────────────────┤
│  PostgreSQL  │  Redis Cache  │  RabbitMQ  │  Keycloak Auth  │
└─────────────────────────────────────────────────────────────┘
```

**Visual Reference:** See `ModularMonolith.drawio.png` for detailed architecture diagram

---

## Module Structure

### Current Modules

**1. Events Module** - Manages event creation and lifecycle
- Entities: Event
- Responsibilities: Event creation, updates, venue management, pricing
- Integration: Publishes `EventUpserted` messages, consumes `EventSoldOut` messages

**2. Tickets Module** - Handles ticket sales and inventory
- Responsibilities: Ticket reservation, purchase, inventory tracking
- Integration: Publishes `EventSoldOut` messages when tickets depleted

**3. Keycloak.Users Module** - Thin integration layer with Keycloak
- Responsibilities: User registration events, authentication integration
- Integration: Publishes `UserRegistered` messages
- Note: Intentionally minimal - delegates authentication/authorization to external Keycloak service

### Module Organization Pattern

Each module follows a consistent directory structure:

```
Modules/
├── Events/
│   ├── Domain.Events/                    # Pure business logic
│   ├── Application.Events/               # Commands, queries, use cases
│   ├── Infrastructure.Events/            # Persistence, external services
│   ├── Controllers.Events/               # HTTP endpoints (primary adapters)
│   ├── Integration.Events.Messaging/     # Integration contracts
│   └── Testing/
│       ├── Testing.Unit.Events/          # Domain & application tests
│       ├── Testing.Integration.Events/   # Infrastructure tests
│       └── Testing.Architecture.Events/  # Boundary enforcement tests
├── Tickets/
│   └── [Same structure as Events]
└── Keycloak.Users/
    └── [Same structure - intentionally thin module]
```

**Key Principle:** Each module is independently testable and has clear boundaries enforced by architecture tests.

---

## Layering Within Modules

Each module follows hexagonal architecture with clear separation:

### 1. Domain Layer (`Domain.{Module}`)

**Purpose:** Pure business logic with zero infrastructure dependencies

**Contains:**
- Entities (e.g., `Event`, `Ticket`)
- Value Objects (e.g., `EventName`, `Venue`)
- Domain Events
- Business Rules
- Contracts (e.g., `IPersistEvents`)

**Example:**
```csharp
// Modules/Events/Domain.Events/Entities/Event.cs
public class Event : Entity, IAmAnAggregateRoot
{
    public EventName EventName { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public Venue Venue { get; private set; }
    public decimal Price { get; private set; }

    // Pure business logic - no infrastructure
}
```

**Dependencies:** References `CommonLibraries/Domain` only

**See:** `Modules/Events/Domain.Events/Domain.Events.csproj:10`

---

### 2. Application Layer (`Application.{Module}`)

**Purpose:** Orchestrates use cases, implements CQRS pattern

**Contains:**
- Commands (write operations)
- Queries (read operations)
- Integration Message Consumers
- Domain Event Handlers

**Example:**
```csharp
// Modules/Events/Application.Events/Commands/EventCommands.cs
public class EventCommands
{
    // Command: Create new event (write side)
    public async Task<Guid> CreateEvent(
        EventName name,
        DateTimeOffset start,
        DateTimeOffset end,
        decimal price)
    {
        // Business logic orchestration
    }
}

// Modules/Events/Application.Events/Queries/EventQueries.cs
public class EventQueries
{
    // Query: Retrieve events (read side)
    public async Task<IList<Event>> GetEvents()
    {
        // Optimized read operations
    }
}
```

**Pattern:** Hybrid CQRS separates command and query responsibilities

**Dependencies:** References Domain layer + `CommonLibraries/Application`

**Cross-Module Integration:** Can reference other modules' `Integration.*.Messaging` contracts

**See:** `Modules/Events/Application.Events/Application.Events.csproj:10-11`

---

### 3. Infrastructure Layer (`Infrastructure.{Module}`)

**Purpose:** Implements persistence, external services, and infrastructure concerns

**Contains:**
- EF Core DbContext implementations
- Repository implementations
- Database configurations
- Integration message publishers

**Dependencies:** References Application + Domain + `CommonLibraries/Infrastructure`

**See:** `Modules/Events/Infrastructure.Events/Infrastructure.Events.csproj:10-14`

---

### 4. Controllers Layer (`Controllers.{Module}`)

**Purpose:** HTTP/REST API adapters (primary adapters in hexagonal architecture)

**Contains:**
- API Controllers
- Request/Response DTOs
- Route definitions

**Example:**
```csharp
// Modules/Events/Controllers.Events/EventController.cs
[ApiController]
public class EventController(EventCommands commands, EventQueries queries)
{
    [HttpGet(Routes.Events)]
    public async Task<IList<Event>> GetEvents()
        => await queries.GetEvents();

    [Authorize(Roles = Roles.Admin)]
    [HttpPost(Routes.Events)]
    public async Task<CreatedResult> CreateEvent([FromBody] EventPayload payload)
    {
        var id = await commands.CreateEvent(/*...*/);
        return Created($"/{Routes.Events}/{id}", id);
    }
}
```

**Pattern:** Thin adapters delegate to Application layer (Commands/Queries)

**See:** `Modules/Events/Controllers.Events/EventController.cs:12`

---

### 5. Integration.Messaging Layer (`Integration.{Module}.Messaging`)

**Purpose:** Define integration contracts between modules (anti-corruption layer)

**Contains:**
- Integration message contracts (immutable records)
- Shared DTOs for cross-module communication

**Example:**
```csharp
// Modules/Events/Integration.Events.Messaging/EventUpserted.cs
public record EventUpserted
{
    public Guid Id { get; init; }
    public string EventName { get; init; } = null!;
    public DateTimeOffset StartDate { get; init; }
    public Venue Venue { get; init; }
    public decimal Price { get; init; }
}
```

**Pattern:** Published via RabbitMQ when events are created/updated

**Cross-Module Usage:** Other modules consume these messages without direct dependencies on the publishing module's internals

**Example Consumer:**
```csharp
// Modules/Tickets/Application.Tickets/IntegrationMessageConsumers/EventUpsertedConsumer.cs
public class EventUpsertedConsumer : IConsumer<EventUpserted>
{
    // React to events being created/updated
}
```

**See:** `Modules/Events/Integration.Events.Messaging/EventUpserted.cs:5`

---

### 6. Testing Layers

**Purpose:** Comprehensive testing at multiple levels

**Structure:**
- `Testing.Unit.{Module}` - Fast, isolated tests of domain/application logic
- `Testing.Integration.{Module}` - Tests with real infrastructure (Testcontainers)
- `Testing.Architecture.{Module}` - Enforce module boundaries and dependencies

**See:** [Testing Strategy](#testing-strategy)

---

## Common Libraries

Shared code reused across all modules:

### CommonLibraries/Domain

**Purpose:** Base classes and patterns for domain modeling

**Contains:**
- `Entity` - Base class with domain events support
- `IAmAnAggregateRoot` - Marker interface for aggregates
- `IAmADomainEvent` - Domain event contract
- Shared value objects (e.g., `Venue`)

**Example:**
```csharp
// CommonLibraries/Domain/Entity.cs
public abstract class Entity(Guid Id)
{
    public Guid Id { get; } = Id;
    private readonly List<IAmADomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IAmADomainEvent> DomainEvents
        => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IAmADomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
}
```

**See:** `CommonLibraries/Domain/Entity.cs:5`

---

### CommonLibraries/Application

**Purpose:** Application-level patterns and contracts

**Contains:**
- `IAmADomainEventHandler` - Contract for domain event handlers
- Application service base classes
- Common use case patterns

**See:** `CommonLibraries/Application/Contracts/IAmADomainEventHandler.cs`

---

### CommonLibraries/Infrastructure

**Purpose:** Infrastructure implementations and patterns

**Contains:**
- `UnitOfWorkDbContext` - EF Core base with transaction support
- `DomainEventsDispatcher` - Publishes domain events after persistence
- `DomainEventsAccessor` - Collects domain events from entities
- Database configuration utilities

**Pattern:** Infrastructure handles domain events → integration messages pipeline

**See:** `CommonLibraries/Infrastructure/DomainEventsDispatching/`

---

### CommonLibraries/Testing

**Purpose:** Shared testing utilities

**Contains:**
- Test data builders
- Common test fixtures
- Testcontainers configurations

---

### CommonLibraries/Keycloak

**Purpose:** Keycloak authentication/authorization integration

**Contains:**
- JWT token handling
- Role definitions (e.g., `Roles.Admin`)
- Keycloak client configuration

---

## Technology Stack

### Runtime Platform
- **.NET 9.0** - Latest LTS release
- **ASP.NET Core** - Web API hosting

### Database
- **PostgreSQL 9.0.4** - Primary data store (via Npgsql + EF Core)
- **Redis 2.9.32** - Distributed caching (via StackExchange.Redis)

### Messaging
- **RabbitMQ 7.2.0** - Message broker for async module communication
- **MassTransit 8.5.5** - Messaging abstraction and integration patterns

### Authentication
- **Keycloak** - Identity and access management
- **JWT Bearer Tokens** - API authentication

### Observability
- **OpenTelemetry 1.13.1** - Distributed tracing and metrics
- **Console Exporter** - Development observability
- **OTLP Exporter** - Production telemetry

### Testing
- **NUnit 4.4.0** - Test framework
- **Shouldly 9.0.0** - Assertion library
- **NSubstitute 5.3.0** - Mocking framework
- **Testcontainers 4.8.1** - Real infrastructure for integration tests
  - PostgreSQL, Redis, RabbitMQ, Keycloak containers

### Development Tools
- **.NET Aspire 9.5.2** - Local development orchestration
- **dbup-postgresql 6.0.3** - Database migrations

### Health Checks
- **AspNetCore.HealthChecks.NpgSql** - PostgreSQL health
- **AspNetCore.HealthChecks.Rabbitmq** - RabbitMQ health
- **AspNetCore.HealthChecks.Redis** - Redis health

**See:** `Directory.Packages.props` for complete package list

---

## Module Communication Patterns

### 1. Asynchronous Communication (Primary Pattern)

**Mechanism:** Domain Events → Integration Messages via RabbitMQ

**Flow:**
1. Business operation completes (e.g., Event created)
2. Domain raises domain event
3. Infrastructure publishes integration message to RabbitMQ
4. Other modules consume messages and react

**Example:**

```csharp
// Events module: Publish when event created
// Infrastructure.Events publishes EventUpserted to RabbitMQ

// Tickets module: React to event changes
public class EventUpsertedConsumer : IConsumer<EventUpserted>
{
    public async Task Consume(ConsumeContext<EventUpserted> context)
    {
        // Update local read model with event details
        // Tickets module now has event information for validation
    }
}
```

**Integration Messages:**
- `EventUpserted` (Events → Tickets)
- `EventSoldOut` (Tickets → Events)
- `UserRegistered` (Keycloak.Users → other modules)

**Benefits:**
- Loose coupling between modules
- Eventual consistency
- Resilient to module failures
- Clear integration contracts

**See:** `Modules/*/Integration.*.Messaging/`

---

### 2. Module Boundaries and Dependencies

**Dependency Rules:**
1. Modules should NOT directly reference other modules' Domain/Application/Infrastructure layers
2. Modules CAN reference other modules' `Integration.*.Messaging` contracts
3. All modules CAN reference Common Libraries

**Example (Events module):**
```xml
<!-- Application.Events.csproj -->
<ItemGroup>
  <!-- ✅ ALLOWED: Reference another module's integration contract -->
  <ProjectReference Include="..\..\Tickets\Integration.Tickets.Messaging\" />

  <!-- ❌ FORBIDDEN: Direct reference to another module's internals -->
  <!-- <ProjectReference Include="..\..\Tickets\Application.Tickets\" /> -->
</ItemGroup>
```

**Enforcement:** Architecture tests verify these boundaries

**See:** `Modules/*/Testing.Architecture.*/` for boundary tests

---

## Testing Strategy

### Test Pyramid

```
                    ┌─────────────┐
                    │ Acceptance  │  ← End-to-end business journeys
                    │   Tests     │
                    └─────────────┘
                  ┌─────────────────┐
                  │   Component     │  ← Module APIs with real infra
                  │     Tests       │
                  └─────────────────┘
              ┌───────────────────────┐
              │   Integration Tests   │  ← Infrastructure integration
              │ (Module-specific)     │
              └───────────────────────┘
          ┌───────────────────────────────┐
          │        Unit Tests             │  ← Domain & application logic
          │    (Fast & Isolated)          │
          └───────────────────────────────┘
      ┌───────────────────────────────────────┐
      │       Architecture Tests              │  ← Enforce boundaries
      └───────────────────────────────────────┘
```

**See:** `ModernTestPyramid.png`

---

### BDD Testing Pattern

**All tests** follow the BDD (Behavior-Driven Development) pattern with separate specifications and step implementations using partial classes.

**Structure:**
- `{Name}.specs.cs` - Test scenarios with Given-When-Then-And
- `{Name}.steps.cs` - Step implementations (private methods)

**Base Classes:**
- `Specification` - Synchronous BDD tests
- `AsyncSpecification` - Asynchronous BDD tests

**See:** `../.claude/docs/testing.md` for complete BDD patterns and examples

---

### 1. Unit Tests (`Testing.Unit.{Module}`)

**Purpose:** Fast, isolated tests of domain logic and business rules

**Scope:**
- Domain entities and value objects
- Business rule validation
- Pure domain logic

**Pattern:** BDD specs + steps with synchronous `Specification` base class

**Tools:**
- NUnit
- Shouldly
- BDD (Testing.Bdd package)

**Example:**

```csharp
// Event.specs.cs
using NUnit.Framework;

namespace Unit;

public partial class EventSpecs
{
    [Test]
    public void an_event_must_have_a_name()
    {
        Scenario(() =>
        {
            Given(valid_inputs);
            And(a_null_user_name);
            When(Validating(creating_an_event));
            Then(Informs("Name cannot be empty"));
        });

        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_event_name);
            When(Validating(creating_an_event));
            Then(Informs("Name cannot be empty"));
        });
    }

    [Test]
    public void cannot_create_event_with_end_date_before_start_date()
    {
        Given(valid_inputs);
        And(an_event_with_end_date_before_start_date);
        When(Validating(creating_an_event));
        Then(Informs("End date cannot be before start date"));
    }

    [Test]
    public void can_create_valid_event()
    {
        Given(valid_inputs);
        When(creating_an_event);
        Then(the_event_is_created);
    }
}
```

```csharp
// Event.steps.cs
using BDD;
using Domain.Events.Entities;
using Domain.Primitives;
using Shouldly;

namespace Unit;

public partial class EventSpecs : Specification
{
    private Guid id;
    private string name = null!;
    private DateTimeOffset start_date = DateTimeOffset.UtcNow.AddDays(1);
    private DateTimeOffset end_date = DateTimeOffset.UtcNow.AddDays(1).AddHours(2);
    private Event user = null!;

    private const string valid_name = "Jackie Chan 123";

    protected override void before_each()
    {
        base.before_each();
        id = Guid.NewGuid();
        name = null!;
        user = null!;
        start_date = DateTimeOffset.UtcNow.AddDays(1);
    }

    private void valid_inputs()
    {
        name = valid_name;
        start_date = DateTimeOffset.UtcNow.AddDays(1);
    }

    private void a_null_user_name()
    {
        name = null!;
    }

    private void an_event_name()
    {
        name = string.Empty;
    }

    private void an_event_with_end_date_before_start_date()
    {
        start_date = DateTimeOffset.UtcNow.AddDays(2);
        end_date = DateTimeOffset.UtcNow.AddDays(1);
    }

    private void creating_an_event()
    {
        user = new Event(id, name, start_date, end_date, Venue.FirstDirectArenaLeeds, 25m);
    }

    private void the_event_is_created()
    {
        user.Id.ShouldBe(id);
        user.EventName.ToString().ShouldBe(valid_name);
        user.StartDate.ShouldBe(start_date);
        user.EndDate.ShouldBe(end_date);
    }
}
```

**Key Patterns:**
- `Scenario()` - Groups related Given-When-Then steps
- `Validating()` - Captures exceptions for validation testing
- `Informs()` - Asserts expected validation message
- `before_each()` - Reset state between tests
- Snake_case method names describe behavior

**Location:** `Modules/{Module}/Testing.Unit.{Module}/`

**See:** `Modules/Events/Testing.Unit.Events/Event.specs.cs`

---

### 2. Integration Tests (`Testing.Integration.{Module}`)

**Purpose:** Test module infrastructure with real dependencies (database, messaging)

**Scope:**
- Database persistence (EF Core with PostgreSQL)
- Message publishing/consuming (RabbitMQ + MassTransit)
- Application layer with real infrastructure
- Domain events → Integration messages flow

**Pattern:** BDD specs + steps with asynchronous operations

**Tools:**
- Testcontainers (PostgreSQL, RabbitMQ)
- MassTransit.Testing (message verification)
- Real infrastructure per test run

**Example:**

```csharp
// EventController.specs.cs
using NUnit.Framework;

namespace Integration;

public partial class EventControllerSpecs
{
    [Test]
    public async Task can_create_event()
    {
              Given(a_request_to_create_an_event);
        await When(creating_the_event);
        await And(requesting_the_event);
              Then(the_event_is_created);
              And(an_integration_event_is_published);
    }

    [Test]
    public async Task cannot_create_event_with_date_in_the_past()
    {
              Given(a_request_to_create_an_event_with_a_date_in_the_past);
        await When(creating_the_event_that_will_fail);
              Then(the_event_is_not_created);
    }

    [Test]
    public async Task cannot_double_book_venue()
    {
        await Given(an_event_exists);
              And(a_request_to_create_an_event_with_the_same_venue_and_time);
        await When(creating_the_event_that_will_fail);
              Then(the_user_is_informed_that_the_venue_is_unavailable);
    }
}
```

**Key Infrastructure:**
- PostgreSQL container for database tests
- RabbitMQ container for messaging tests
- MassTransit test harness for message verification
- Database truncation between tests

**Benefits:**
- Catch persistence issues (EF Core mapping, constraints)
- Verify integration messages are published correctly
- Test complex queries and transactions
- Validate domain events → integration messages pipeline

**Location:** `Modules/{Module}/Testing.Integration.{Module}/`

**See:** `Modules/Events/Testing.Integration.Events/EventController.specs.cs`

---

### 3. Architecture Tests (`Testing.Architecture.{Module}`)

**Purpose:** Enforce architectural boundaries and dependency rules

**Scope:**
- Module isolation (no cross-module dependencies on internals)
- Layer dependencies (Domain → Application → Infrastructure flow)
- DDD patterns (immutability, aggregate boundaries)
- Naming conventions

**Pattern:** BDD specs + steps with NetArchTest.Rules

**Tools:**
- NetArchTest.Rules

**Example:**

```csharp
// Module.specs.cs
namespace Testing.Architecture.Events.Modules;

internal partial class ModuleSpecs
{
    [Test]
    public void domain_layer_should_not_reference_application_layer()
    {
        When(checking_the_domain_layer_for_application_layer_references);
        Then(there_should_be_no_references_to_application_layer);
    }

    [Test]
    public void application_layer_should_not_reference_infrastructure_layer()
    {
        Given(checking_the_application_layer_for_infrastructure_layer_references);
        Then(there_should_be_no_references_to_infrastructure_layer);
    }

    [Test]
    public void infrastructure_layer_should_not_reference_controller_layer()
    {
        Given(checking_the_infrastructure_layer_for_controller_layer_references);
        Then(there_should_be_no_references_to_controller_layer);
    }
}
```

```csharp
// Domain.specs.cs
namespace Testing.Architecture.Events.Domain;

public partial class DomainSpecs
{
    [Test]
    public void domain_events_should_be_immutable()
    {
        Given(domain_event_types);
        Then(should_be_immutable);
    }

    [Test]
    public void entities_that_are_not_aggregate_roots_cannot_be_public()
    {
        Given(entity_types_that_are_not_aggregate_roots);
        Then(should_not_be_public_if_not_aggregate_root);
    }

    [Test]
    public void entity_cannot_have_reference_to_other_aggregate_root()
    {
        Given(entity_types_that_are_aggregate_roots);
        Then(should_not_reference_other_aggregate_root);
    }
}
```

**Enforced Rules:**
- Domain layer cannot reference Application or Infrastructure
- Application layer cannot reference Infrastructure or Controllers
- Infrastructure cannot reference Controllers
- Domain events must be immutable
- Entities cannot reference other aggregate roots (no cross-aggregate references)
- Non-aggregate entities must be internal

**Location:** `Modules/{Module}/Testing.Architecture.{Module}/`

**See:** `Modules/Events/Testing.Architecture.Events/`

---

### 4. Component Tests (`Testing/Testing.Component/`)

**Purpose:** Test module APIs as black boxes with real infrastructure

**Scope:**
- HTTP API contracts (Controllers)
- Authentication/Authorization (Keycloak)
- Module behavior with real database and messaging
- Integration message publication verification
- Cross-cutting concerns (health checks)

**Pattern:** BDD specs + steps with `WebApplicationFactory`

**Tools:**
- Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)
- Testcontainers (PostgreSQL, RabbitMQ, Keycloak)
- MassTransit.Testing (message verification)
- Database truncation between tests

**Example:**

```csharp
// EventApi.specs.cs
using NUnit.Framework;

namespace Component.Api;

public partial class EventApiSpecs
{
    [Test]
    public async Task can_create_event()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_an_event);
        await When(creating_the_event);
        await And(requesting_the_event);
        await Then(the_event_is_created);
              And(an_integration_event_is_published);
    }

    [Test]
    public async Task a_non_admin_cannot_create_event()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_an_event_as_a_non_admin_user);
        await When(creating_the_event_that_should_fail);
              Then(the_event_creation_is_forbidden);
    }

    [Test]
    public async Task an_anonymous_user_can_view_events()
    {
              Given(an_admin_user_exists);
        await And(an_event_exists);
        await When(listing_the_events_as_an_anonymous_user);
        await Then(the_events_are_returned);
    }
}
```

```csharp
// EventApi.steps.cs (excerpt)
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using MassTransit.Testing;

namespace Component.Api;

public partial class EventApiSpecs : TruncateDbSpecification
{
    private IntegrationWebApplicationFactory<Program> factory = null!;
    private HttpClient client = null!;
    private static PostgreSqlContainer database = null!;
    private static RabbitMqContainer rabbit = null!;
    private ITestHarness testHarness = null!;

    protected override async Task before_all()
    {
        database = PostgreSql.CreateContainer();
        await database.StartAsync();
        database.Migrate();

        rabbit = RabbitMq.CreateContainer();
        await rabbit.StartAsync();
    }

    protected override async Task before_each()
    {
        factory = new IntegrationWebApplicationFactory<Program>(
            database.GetConnectionString()
        );
        client = factory.CreateClient();
        testHarness = factory.Services.GetRequiredService<ITestHarness>();

        client.DefaultRequestHeaders.Add(UserHeaders.UserType, nameof(UserType.Admin));
        await testHarness.Start();
    }

    private void an_admin_user_exists() {}

    private async Task creating_the_event()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
        response_code.ShouldBe(HttpStatusCode.Created);
        returned_id = JsonSerialization.Deserialize<Guid>(
            await response.Content.ReadAsStringAsync()
        );
    }

    private void an_integration_event_is_published()
    {
        testHarness.Published.Select<EventUpserted>()
            .Any(e =>
                e.Context.Message.Id == returned_id &&
                e.Context.Message.EventName == name
            ).ShouldBeTrue("Event was not published to the bus");
    }
}
```

**Key Characteristics:**
- Tests module via HTTP API (black box)
- Uses real PostgreSQL + RabbitMQ containers
- Verifies authentication/authorization
- Confirms integration messages published
- Database reset between tests (truncation)

**Difference from Integration Tests:**
- Component: Tests via HTTP API (external boundary)
- Integration: Tests application layer directly (internal)

**Location:** `Testing/Testing.Component/`

**See:** `Testing/Testing.Component/Api/EventApi.specs.cs`

---

### 5. Acceptance Tests (`Testing/Testing.Acceptance/`)

**Purpose:** End-to-end business scenarios across multiple modules

**Scope:**
- Complete user journeys
- Cross-module workflows
- Business-level acceptance criteria
- Full system integration

**Pattern:** BDD specs + steps with full system running

**Tools:**
- WebApplicationFactory (full Host)
- Testcontainers (all infrastructure)
- Real HTTP calls across modules

**Example:**

```csharp
// TicketBuddy.specs.cs
using NUnit.Framework;

namespace Acceptance;

public partial class TicketBuddySpecs
{
    [Test]
    public async Task user_can_purchase_tickets_for_an_event()
    {
        await Given(an_event_exists);
        await And(a_user_exists);
        await And(tickets_are_available_for_the_event);
        await When(the_user_purchases_tickets_for_the_event);
        await Then(the_purchase_is_successful);
    }
}
```

**Key Characteristics:**
- Tests business capabilities, not technical implementation
- Spans multiple modules (Events → Tickets → Users)
- Verifies complete workflows
- Uses domain language in test names

**Difference from Component Tests:**
- Acceptance: Cross-module business journeys
- Component: Single module API testing

**Location:** `Testing/Testing.Acceptance/`

**See:** `Testing/Testing.Acceptance/TicketBuddy.specs.cs`

---

### Testing Infrastructure

**Testcontainers:**
All integration, component, and acceptance tests use Testcontainers for real infrastructure:

```csharp
// PostgreSQL
database = PostgreSql.CreateContainer();
await database.StartAsync();
database.Migrate(); // Run dbup migrations

// RabbitMQ
rabbit = RabbitMq.CreateContainer();
await rabbit.StartAsync();

// Keycloak (component/acceptance only)
keycloak = Keycloak.CreateContainer();
await keycloak.StartAsync();

// Redis (when needed)
redis = Redis.CreateContainer();
await redis.StartAsync();
```

**Database Management:**
- `TruncateDbSpecification` base class provides database cleanup
- Truncates tables between tests (faster than recreating containers)
- Migrations run once per test session

**Message Verification:**
- `ITestHarness` from MassTransit.Testing
- Verifies integration messages published to RabbitMQ
- Inspects message content and metadata

---

### Test Execution

**Run all tests:**
```bash
dotnet test
```

**Run specific test level:**
```bash
# Unit tests only (fast)
dotnet test --filter "FullyQualifiedName~Testing.Unit"

# Integration tests
dotnet test --filter "FullyQualifiedName~Testing.Integration"

# Architecture tests
dotnet test --filter "FullyQualifiedName~Testing.Architecture"

# Component tests
dotnet test --filter "FullyQualifiedName~Testing.Component"

# Acceptance tests
dotnet test --filter "FullyQualifiedName~Testing.Acceptance"
```

**Run specific module tests:**
```bash
dotnet test Modules/Events/Testing.Unit.Events
```

---

### Test Coverage Strategy

**Goal:** 100% coverage through business behavior, not line coverage

**Approach:**
1. **Unit Tests** - Cover all domain rules and validation logic
2. **Integration Tests** - Cover infrastructure concerns and data flow
3. **Architecture Tests** - Enforce structural rules automatically
4. **Component Tests** - Cover API contracts and cross-cutting concerns
5. **Acceptance Tests** - Cover critical business journeys

**What NOT to test:**
- Implementation details (private methods)
- Framework code (ASP.NET, EF Core)
- Third-party libraries
- Trivial property getters/setters

**What TO test:**
- Business rules and validation
- Domain behavior and invariants
- API contracts and responses
- Integration message contracts
- Cross-module workflows
- Architectural boundaries

---

### Test Naming Conventions

**Test Methods:**
- Describe behavior in lowercase with underscores
- Start with context, followed by expected outcome
- Examples:
  - `an_event_must_have_a_name()`
  - `cannot_create_event_with_end_date_before_start_date()`
  - `user_can_purchase_tickets_for_an_event()`

**Step Methods:**
- Snake_case describing state or action
- Given: `valid_inputs()`, `an_event_exists()`
- When: `creating_the_event()`, `purchasing_tickets()`
- Then: `the_event_is_created()`, `the_purchase_is_successful()`

**See:** `../.claude/docs/testing.md` for complete naming guidelines

---

## Development Workflow

### Local Development Setup

**Prerequisites:**
- .NET 9.0 SDK
- Docker (for Aspire orchestration)

**Run Locally:**
```bash
# Start all infrastructure via .NET Aspire
cd LocalHosting/Host.Aspire
dotnet run

# Or use docker-compose
cd LocalHosting
docker-compose up
```

**.NET Aspire** orchestrates:
- PostgreSQL (database)
- Redis (cache)
- RabbitMQ (messaging)
- Keycloak (auth)
- TicketBuddy API (Host)

**See:** `LocalHosting/Host.Aspire/Program.cs`

---

### Database Migrations

**Tool:** dbup-postgresql

**Location:** `Database/Migrations/` (SQL scripts)

**Run Migrations:**
```bash
cd Database/Host.Migrations
dotnet run
```

**See:** `Database/Host.Migrations/Program.cs`

---

### TDD Workflow

**Principle:** Test-Driven Development is non-negotiable

**Process:**
1. **RED** - Write failing test describing desired behavior
2. **GREEN** - Write minimum code to make test pass
3. **REFACTOR** - Improve design while keeping tests green

**See:** `../.claude/docs/workflow.md` for complete TDD guidelines

---

## Architectural Decisions

### Why Modular Monolith?

**Benefits:**
- **Organizational clarity** - Clear module boundaries and responsibilities
- **Simplified deployment** - Single deployment unit
- **Shared infrastructure** - No distributed transaction complexity
- **Development velocity** - Faster iteration without service orchestration
- **Migration path** - Can extract modules to microservices later if needed

**Trade-offs:**
- **Shared database** - Modules share PostgreSQL instance (acceptable for now)
- **Coupled deployment** - All modules deploy together (simplifies early development)
- **Vertical scaling only** - Cannot independently scale modules (not a concern yet)

---

### Why Hexagonal Architecture?

**Benefits:**
- **Testability** - Core logic testable without infrastructure
- **Flexibility** - Can swap infrastructure (e.g., PostgreSQL → MongoDB) without changing core
- **Clear boundaries** - Adapters vs. core logic separation
- **Framework independence** - Core doesn't depend on ASP.NET, EF Core, etc.

**See:** `../.claude/docs/architecture.md` for hexagonal architecture details

---

### Why CQRS?

**Benefits:**
- **Optimized reads** - Queries can bypass domain model for performance
- **Optimized writes** - Commands focus on business rules and validation
- **Scalability** - Read and write models can evolve independently
- **Clarity** - Intent clear (Command vs. Query)

**Pattern:** Hybrid CQRS (shared database, separate models)

---

### Why Integration Messages?

**Benefits:**
- **Loose coupling** - Modules don't know about each other's internals
- **Resilience** - RabbitMQ provides buffering and retry
- **Auditability** - Message history provides event log
- **Evolution** - Modules can evolve independently

**Pattern:** Event-driven architecture within monolith

---

## Reference Documentation

**Project Documentation:**
- Architecture principles: `../.claude/docs/architecture.md`
- Testing guidelines: `../.claude/docs/testing.md`
- TDD workflow: `../.claude/docs/workflow.md`
- Code style: `../.claude/docs/code-style.md`
- DDD summary: `../.claude/docs/ddd-summary.md`

**Visual References:**
- Modular Monolith diagram: `ModularMonolith.drawio.png`
- Clean Architecture diagram: `CleanArchitecture.jpg`
- Test Pyramid: `ModernTestPyramid.png`
- Observability: `Observability.png`

**Key Files:**
- Host entry point: `../Host/Program.cs:4`
- Package versions: `../Directory.Packages.props`
- Solution structure: `../TicketBuddy.sln`

---

## Glossary

**Module** - Self-contained business capability with clear boundaries (Events, Tickets, Keycloak.Users)

**Aggregate Root** - Entity that enforces consistency boundaries (implements `IAmAnAggregateRoot`)

**Domain Event** - Something that happened in the domain (e.g., EventCreated)

**Integration Message** - Contract between modules published via RabbitMQ (e.g., EventUpserted)

**CQRS** - Command Query Responsibility Segregation - separate read and write models

**Hexagonal Architecture** - Ports and Adapters pattern isolating core from infrastructure

**Primary Adapter** - Drives the application (HTTP controllers, message consumers)

**Secondary Adapter** - Driven by application (repositories, external service clients)

**Testcontainers** - Library for running real infrastructure (DB, messaging) in Docker for tests