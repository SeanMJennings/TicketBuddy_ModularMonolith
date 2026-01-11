# C# Language Conventions

Concise C#-specific syntax and language feature preferences for this codebase.

## Modern C# Features

### Primary Constructors (Preferred)

```csharp
// ✅ Preferred
public class TicketRepository(TicketDbContext dbContext) : IPersistTickets
{
    public async Task<Ticket> Get(Guid id) => await dbContext.Tickets.FindAsync(id);
}

// ❌ Avoid
public class TicketRepository : IPersistTickets
{
    private readonly TicketDbContext _dbContext;
    public TicketRepository(TicketDbContext dbContext) => _dbContext = dbContext;
}
```

### Collection Expressions

```csharp
// ✅ Preferred
List<string> errors = [];
Guid[] ids = [id1, id2, id3];

// ❌ Avoid
List<string> errors = new List<string>();
Guid[] ids = new Guid[] { id1, id2, id3 };
```

### Target-Typed `new()`

```csharp
// ✅ Preferred - when type is apparent
private readonly List<IDescribeADomainEvent> _domainEvents = new();
Money price = new(100m);

// ✅ OK - when type adds clarity
var theEvent = new Event(id, name, start, end, venueId, price);
```

### File-Scoped Namespaces

```csharp
// ✅ Preferred
namespace Domain.Tickets.Ticket;

public class Ticket { }

// ❌ Avoid
namespace Domain.Tickets.Ticket
{
    public class Ticket { }
}
```

## Type Preferences

### Domain Events → `readonly record struct`

```csharp
public readonly record struct AllTicketsSold(Guid EventId) : IDescribeADomainEvent;
public readonly record struct EventCreated(Guid EventId, string Name) : IDescribeADomainEvent;
```

### Value Objects → `readonly struct`

```csharp
public readonly struct EventName : IEquatable<EventName>
{
    private readonly StringValueObject<EventName> _value;
    public EventName(string name) { _value = new(name); }
    // ...equality members
}
```

### Value Objects (Simple) → `readonly record struct`

```csharp
public readonly record struct Money
{
    private decimal Amount { get; }
    public Money(decimal amount) { Amount = amount; }
}
```

### DTOs/Payloads → `record`

```csharp
public record TicketPurchasePayload(Guid[] TicketIds);
public record EventSoldOut { public Guid EventId { get; init; } }
```

## Modifier Preferences

### Visibility: Prefer `private` and `internal`

```csharp
// ✅ Only use public when required (API surface, interface implementations)
public class GetTicketsEndpoint : ControllerBase { }  // Must be public for ASP.NET

// ✅ Internal for implementation details
internal class TicketValidator { }

// ✅ Private for class internals
private readonly List<IDescribeADomainEvent> _domainEvents = [];
```

### Interface Members: Omit Redundant `public`

```csharp
// ✅ Preferred
public interface IPersistTickets
{
    Task<Ticket?> Get(Guid id);
    Task Add(Ticket ticket);
}

// ❌ Avoid
public interface IPersistTickets
{
    public Task<Ticket?> Get(Guid id);
    public Task Add(Ticket ticket);
}
```

## Expression-Bodied Members

### One-Liners → Expression Body

```csharp
// ✅ Preferred for simple members
public bool IsAvailable => UserId is null;
public override string ToString() => _value.ToString();
public void UpdatePrice(Money price) => Price = price;
public static implicit operator string(EventName e) => e._value;

// ✅ Multi-line logic → block body
public void Purchase(Guid userId)
{
    if (UserId is not null) throw new ValidationException("Ticket not available");
    UserId = userId;
    PurchasedAt = DateTimeOffset.UtcNow;
}
```

## Control Flow

### One-Liner If → No Braces

```csharp
// ✅ Preferred for single statements
if (!IsAvailable) return;
if (tickets.Count == 0) throw new ValidationException("No tickets");

// ✅ Multi-line → use braces
if (endDate < startDate)
{
    throw new ValidationException("End date cannot be before start date");
}
```

## Nullable Reference Types & Pattern Matching

### Pattern Matching Preferred

```csharp
// ✅ Preferred
if (userId is null) return;
if (ticket is not null) Process(ticket);
if (obj is EventName other) return _value.Equals(other._value);

// ❌ Avoid
if (userId == null) return;
if (ticket != null) Process(ticket);
```

### Nullable Annotations

```csharp
// ✅ Use nullable annotations
public async Task<Event?> Get(Guid id);
public Guid? UserId { get; private set; }
```

## Formatting

### `var` Usage

```csharp
// ✅ Use var when type is apparent
var tickets = await repository.GetByEventId(eventId);
var eventId = Guid.NewGuid();

// ✅ Explicit type when it adds clarity
IReadOnlyList<Ticket> tickets = await repository.GetByIds(ids);
```

### No Trailing Newline

Files should not end with an empty line.

## Summary

| Feature | Preference |
|---------|------------|
| Constructors | Primary constructors |
| Collections | `[]` expression syntax |
| Object creation | `new()` when type apparent |
| Namespaces | File-scoped |
| Domain events | `readonly record struct` |
| Value objects | `readonly struct` or `readonly record struct` |
| DTOs | `record` |
| Visibility | Prefer `private`/`internal` |
| Interface members | Omit `public` |
| One-liners | Expression-bodied, no braces on `if` |
| Null checks | Pattern matching (`is null`) |
| Type inference | `var` when apparent |
