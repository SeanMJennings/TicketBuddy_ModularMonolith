# Testing Standards

## Testing Philosophy

**Core principle**: Black-box behavioral testing with real dependencies. Mock only at application edges.

### Key Tenets

1. **Test behavior, not implementation** - Tests describe what the system does, not how it does it
2. **Favor real dependencies** - Use Testcontainers for databases, message brokers, and identity providers
3. **Mock only at true boundaries** - External APIs, payment gateways, email services - things you don't control
4. **100% coverage through business behavior** - Every line is covered because it implements tested behavior, not because we wrote tests targeting lines

### What This Means in Practice

```
PREFER: Real PostgreSQL in a container
AVOID:  Mocking IDbConnection or repository interfaces

PREFER: Real RabbitMQ with MassTransit test harness
AVOID:  Mocking IMessageBus

PREFER: Real Keycloak for auth testing
AVOID:  Mocking IAuthenticationService

MOCK:   External payment provider (Stripe, PayPal)
MOCK:   Third-party email service (SendGrid)
MOCK:   External APIs you don't control
```

### Why Real Dependencies?

- **Catch real bugs**: EF Core mappings, SQL constraints, message serialization
- **Confidence**: Tests prove the system works, not that mocks behave as expected
- **Refactoring safety**: Change internals freely without updating mock setups
- **Documentation**: Tests show actual system behavior

---

## Test Pyramid

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
      │       Architecture Tests              │  ← Enforce domain qualities
      └───────────────────────────────────────┘
```

### When to Use Each Level

| Level | Purpose | Infrastructure | Speed |
|-------|---------|----------------|-------|
| **Unit** | Domain logic, validation, business rules | None | Fast |
| **Integration** | Database, messaging within a module | Testcontainers | Medium |
| **Component** | HTTP API contracts, auth, cross-cutting | Testcontainers + WebApplicationFactory | Medium |
| **Acceptance** | Cross-module business journeys | Full system | Slower |
| **Architecture** | Domain immutability, aggregate boundaries | None (reflection-based) | Fast |

---

## BDD Testing Framework

All tests use the `Testing.Bdd` package with separate specifications and step implementations using partial classes.

### File Organization

```
{Name}.specs.cs    // Test scenarios with Given-When-Then
{Name}.steps.cs    // Step implementations (private methods)
```

Tests are organized by endpoint/behavior, with specs and steps as partial classes in separate files. In Visual Studio/Rider, nest the steps file under the specs file for cleaner navigation.

**Nesting via .csproj:**

Add `DependentUpon` entries in the test project file to nest steps under specs:

```xml
<ItemGroup>
  <Compile Update="Events\CreateEvent.steps.cs">
    <DependentUpon>CreateEvent.specs.cs</DependentUpon>
  </Compile>
  <Compile Update="Events\GetEventById.steps.cs">
    <DependentUpon>GetEventById.specs.cs</DependentUpon>
  </Compile>
</ItemGroup>
```

This makes the Solution Explorer show:
```
Events/
├── CreateEvent.specs.cs
│   └── CreateEvent.steps.cs  (nested)
├── GetEventById.specs.cs
│   └── GetEventById.steps.cs (nested)
```

### Base Classes

#### Specification (Synchronous)

For unit tests and architecture tests - no I/O, pure in-memory logic.

```csharp
public abstract class Specification
{
    // Lifecycle hooks
    protected virtual void before_each() { }
    protected virtual void after_each() { }

    // BDD steps - all take Action (synchronous)
    protected void Given(Action action) { action(); }
    protected void When(Action action) { action(); }
    protected void Then(Action action) { action(); }
    protected void And(Action action) { action(); }

    // Multiple scenarios in one test
    protected void Scenario(Action test);

    // Validation helpers
    protected Action Validating(Action action);  // Captures exceptions
    protected Action Informs(string message);    // Asserts exception message
}
```

#### AsyncSpecification (Asynchronous)

For integration, component, and acceptance tests - I/O with databases, HTTP, messaging.

```csharp
public abstract class AsyncSpecification
{
    // Lifecycle hooks
    protected virtual Task before_all() => Task.CompletedTask;
    protected virtual Task before_each() => Task.CompletedTask;
    protected virtual Task after_each() => Task.CompletedTask;
    protected virtual Task after_all() => Task.CompletedTask;

    // BDD steps - BOTH sync and async overloads
    protected void Given(Action action) { action(); }
    protected Task Given(Func<Task> action) => action();

    protected void When(Action action) { action(); }
    protected Task When(Func<Task> action) => action();

    protected void Then(Action action) { action(); }
    protected Task Then(Func<Task> action) => action();

    protected void And(Action action) { action(); }
    protected Task And(Func<Task> action) => action();
}
```

### The Alignment Pattern

**Key insight**: `AsyncSpecification` provides both sync and async overloads. Use `await` only for actual I/O operations. The visual alignment makes sync vs async immediately clear:

```csharp
[Test]
public async Task can_create_event()
{
          Given(a_request_to_create_an_event);   // sync - builds request object
    await When(creating_the_event);              // async - HTTP/database call
    await And(requesting_the_event);             // async - HTTP call
          Then(the_event_is_created);            // sync - in-memory assertion
          And(an_integration_event_is_published); // sync - checks test harness
}
```

Step implementations match their usage:

```csharp
// Sync steps - return void
private void a_request_to_create_an_event()
{
    eventPayload = new EventPayload(name, startDate, endDate, venueId, price);
}

private void the_event_is_created()
{
    theEvent.Id.ShouldBe(returned_id);
    theEvent.EventName.ToString().ShouldBe(name);
}

// Async steps - return Task
private async Task creating_the_event()
{
    var response = await createEventEndpoint.CreateEvent(eventPayload);
    returned_id = (Guid)response.Value!;
}

private async Task requesting_the_event()
{
    theEvent = (await getEventByIdEndpoint.GetEvent(returned_id)).Value!;
}
```

---

## Test Types

### 1. Unit Tests

**Purpose**: Fast, isolated tests of domain logic and business rules.

**Location**: `Modules/{Module}/Testing.Unit.{Module}/`

**Base class**: `Specification`

**What to test**:
- Domain entities and value objects
- Business rule validation
- Pure domain logic

**Example**:

```csharp
// Event.specs.cs
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
            Then(Informs("EventName cannot be null or empty"));
        });

        Scenario(() =>
        {
            Given(valid_inputs);
            And(an_event_name);
            When(Validating(creating_an_event));
            Then(Informs("EventName cannot be null or empty"));
        });
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
public partial class EventSpecs : Specification
{
    private Guid id;
    private string name = null!;
    private Event result = null!;

    protected override void before_each()
    {
        base.before_each();
        id = Guid.NewGuid();
        name = null!;
        result = null!;
    }

    private void valid_inputs() => name = "Concert 2024";
    private void a_null_user_name() => name = null!;
    private void creating_an_event() => result = new Event(id, name, ...);
    private void the_event_is_created() => result.Id.ShouldBe(id);
}
```

### 2. Integration Tests

**Purpose**: Test module infrastructure with real dependencies.

**Location**: `Modules/{Module}/Testing.Integration.{Module}/`

**Base class**: `TruncateDbSpecification` (extends `AsyncSpecification`)

**Infrastructure**: PostgreSQL + RabbitMQ via Testcontainers

**What to test**:
- Database persistence (EF Core with PostgreSQL)
- Message publishing/consuming (RabbitMQ + MassTransit)
- Domain events → Integration messages flow

**Example**:

```csharp
// EventController.specs.cs
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
    public async Task cannot_double_book_venue()
    {
        await Given(an_event_exists);
              And(a_request_to_create_an_event_with_the_same_venue_and_time);
        await When(creating_the_event_that_will_fail);
              Then(the_user_is_informed_that_the_venue_is_unavailable);
    }
}
```

### 3. Component Tests

**Purpose**: Test module APIs as black boxes with real infrastructure.

**Location**: `Testing/Testing.Component/`

**Base class**: `TruncateDbSpecification`

**Infrastructure**: WebApplicationFactory + Testcontainers (PostgreSQL, RabbitMQ, Keycloak)

**What to test**:
- HTTP API contracts
- Authentication/Authorization
- Cross-cutting concerns

**Key difference from Integration**: Tests via HTTP API (external boundary), not application layer directly.

### 4. Acceptance Tests

**Purpose**: End-to-end business scenarios across multiple modules.

**Location**: `Testing/Testing.Acceptance/`

**Base class**: `AsyncSpecification` or `TruncateDbSpecification`

**Infrastructure**: Full system with all containers

**What to test**:
- Complete user journeys
- Cross-module workflows
- Business-level acceptance criteria

### 5. Architecture Tests

**Purpose**: Enforce domain qualities via reflection.

**Location**: `Modules/{Module}/Testing.Architecture.{Module}/`

**Base class**: `Specification`

**What to test**:
- Domain events are immutable
- Domain primitives (value objects) are immutable
- Non-aggregate-root entities are internal
- Entities don't reference other aggregate roots

**Note**: Layer dependencies are NOT tested here. They're enforced at compile-time via Roslyn analyzers in the `Architecture.{Module}` projects.

**Example**:

```csharp
// Domain.specs.cs
public partial class DomainSpecs
{
    [Test]
    public void domain_events_should_be_immutable()
    {
        Given(domain_event_types);
        Then(should_be_immutable);
    }

    [Test]
    public void domain_primitives_should_be_immutable()
    {
        Given(domain_primitives);
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

```csharp
// Domain.steps.cs
public partial class DomainSpecs : Specification
{
    private IEnumerable<Type> types = [];
    private static Assembly DomainAssembly => typeof(Event).Assembly;

    private void domain_event_types()
    {
        types = DomainAssembly.GetTypes()
            .Where(t => typeof(IDescribeADomainEvent).IsAssignableFrom(t)
                     && t != typeof(IDescribeADomainEvent));
    }

    private void should_be_immutable()
    {
        List<Type> failingTypes = [];
        foreach (var type in types)
        {
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var hasWritableField = fields.Any(f => f is { IsInitOnly: false, IsLiteral: false });
            if (hasWritableField)
                failingTypes.Add(type);
        }
        Assert.That(failingTypes, Is.Null.Or.Empty);
    }
}
```

---

## Testing Infrastructure

### Testcontainers Setup

```csharp
protected override async Task before_all()
{
    database = PostgreSql.CreateContainer();
    await database.StartAsync();
    database.Migrate();
}

protected override async Task after_all()
{
    await database.StopAsync();
    await database.DisposeAsync();
}
```

### Database Management

- `TruncateDbSpecification` base class provides cleanup between tests
- Truncation is faster than recreating containers
- Migrations run once per test session via `before_all()`

### Message Verification

```csharp
private ITestHarness testHarness;

private void an_integration_event_is_published()
{
    testHarness.Published.Select<EventUpserted>()
        .Any(e =>
            e.Context.Message.Id == returned_id &&
            e.Context.Message.EventName == name)
        .ShouldBeTrue("Event was not published to the bus");
}
```

---

## Test Naming Conventions

### Test Methods

Describe behavior in lowercase with underscores:

```csharp
an_event_must_have_a_name()
cannot_create_event_with_end_date_before_start_date()
user_can_purchase_tickets_for_an_event()
```

### Step Methods

Snake_case describing state or action:

```csharp
// Given
valid_inputs()
an_event_exists()
a_request_to_create_an_event()

// When
creating_the_event()
creating_the_event_that_will_fail()

// Then
the_event_is_created()
the_user_is_informed_that_the_venue_is_unavailable()
```

---

## What NOT to Test

- Implementation details (private methods)
- Framework code (ASP.NET, EF Core internals)
- Third-party libraries
- Trivial property getters/setters

## What TO Test

- Business rules and validation
- Domain behavior and invariants
- API contracts and responses
- Integration message contracts
- Cross-module workflows
- Domain qualities (immutability, aggregate boundaries)

---

## Test Execution

```bash
# All tests
dotnet test

# By level
dotnet test --filter "FullyQualifiedName~Testing.Unit"
dotnet test --filter "FullyQualifiedName~Testing.Integration"
dotnet test --filter "FullyQualifiedName~Testing.Component"
dotnet test --filter "FullyQualifiedName~Testing.Acceptance"
dotnet test --filter "FullyQualifiedName~Testing.Architecture"

# Specific module
dotnet test Modules/Events/Testing.Unit.Events
```