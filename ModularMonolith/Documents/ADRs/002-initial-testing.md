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
        user.Id.Should().Be(id);
        user.EventName.ToString().Should().Be(valid_name);
        user.StartDate.Should().Be(start_date);
        user.EndDate.Should().Be(end_date);
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