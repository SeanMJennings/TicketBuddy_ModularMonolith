# ADR-004: Cross-Module Data Minimization Strategy

**Date**: 2026-01-04

**Tags**: modular-monolith, integration, messaging, cross-module, coupling

## Context

In a modular monolith architecture, modules need to share information about aggregates owned by other modules. This creates a fundamental tension:

**The Problem:**
- Modules are autonomous and own their aggregates (e.g., Events module owns Venue aggregate)
- Other modules need some information about these aggregates (e.g., Tickets module needs venue details to display)
- Sharing too much data couples modules to implementation details
- Sharing too little data may require synchronous queries or additional messages

**Specific Case - Venue Synchronization:**
The Events module owns the Venue aggregate with these properties:
- `Id` (Guid)
- `Name` (VenueName value object)
- `Address` (Address value object: Street, City, Postcode)
- `Capacity` (uint: 1-50 seats)

The Tickets module needs venue information to:
- Display venue name when showing available tickets
- Validate ticket quantity doesn't exceed venue capacity
- Show capacity information to users

**Question:** What data should `VenueUpserted` message contain?

**Options:**
1. Full aggregate (Id, Name, Address, Capacity)
2. Minimal data (Id, Name, Capacity)
3. Just ID (Tickets module queries Events module for details)
4. Shared database read access

**Requirements:**
- Maintain module autonomy
- Support asynchronous communication pattern
- Enable Tickets module to fulfill its responsibilities
- Avoid coupling to Events module's internal structure
- Support future evolution of both modules

## Decision

We will send **only minimal required data** in cross-module integration messages.

**Pattern:**
```csharp
// Events module: Integration message
public record VenueUpserted
{
    public Guid Id { get; init; }           // ✅ Required (identity)
    public string Name { get; init; }       // ✅ Required (display)
    public uint Capacity { get; init; }     // ✅ Required (validation)
    // ❌ Address NOT included - not needed by consuming modules
}
```

**Consuming Module (Tickets) Read Model:**
```csharp
// Tickets module: Read model entity
public class Venue : Entity
{
    public string Name { get; set; }
    public uint Capacity { get; set; }
    // Only stores what it needs from VenueUpserted
}
```

**Decision Framework for What to Include:**

Include a property in an integration message IF:
- ✅ Consuming module needs it to fulfill business requirements
- ✅ Data is relatively stable (doesn't change frequently)
- ✅ Property is part of the aggregate's public contract

Exclude a property from an integration message IF:
- ❌ No consuming module currently needs it
- ❌ Property is an implementation detail of the owning module
- ❌ Including it would create unnecessary coupling
- ❌ Property can be derived or computed by consumer

## Alternatives Considered

### Alternative 1: Full Aggregate Synchronization

**Approach:**
```csharp
public record VenueUpserted
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public uint Capacity { get; init; }
    public AddressDto Address { get; init; }  // Include everything
}
```

**Pros:**
- Consumers have complete information
- Reduces need for future messages if requirements change
- No ambiguity about what's available

**Cons:**
- Creates coupling to internal structure (Address value object)
- Tickets module doesn't need Address
- If Address structure changes, Tickets module is affected
- Larger messages on message bus
- Encourages consumers to depend on unnecessary data

**Why Rejected:**
Violates the principle of minimal coupling. The Tickets module has no use case requiring venue address information. Including it creates unnecessary dependency on the Events module's internal Address value object structure. If we later expand Address to support international venues, we'd have to update Tickets module even though it never used the data.

---

### Alternative 2: ID-Only with Synchronous Queries

**Approach:**
```csharp
public record VenueUpserted
{
    public Guid Id { get; init; }  // Only the ID
}

// Tickets module queries Events module for details when needed
public class GetTicketsForEvent
{
    public async Task Execute(Guid eventId)
    {
        var venue = await eventsModuleClient.GetVenue(venueId);  // Synchronous call
        // Use venue details...
    }
}
```

**Pros:**
- Absolute minimal coupling in messages
- Always gets latest venue data
- No data duplication

**Cons:**
- **Violates async-first pattern** - introduces synchronous cross-module dependency
- **Performance impact** - every read requires cross-module query
- **Availability coupling** - Tickets module can't function if Events module is down
- **Breaks module autonomy** - Tickets module depends on Events module being available
- Requires exposing query endpoints on Events module

**Why Rejected:**
This defeats the purpose of event-driven architecture. One of the key benefits of the modular monolith with async messaging is module resilience - the Tickets module should be able to serve requests even if the Events module is temporarily unavailable (e.g., during deployment). Synchronous queries couple module availability and create a distributed monolith anti-pattern.

---

### Alternative 3: Shared Database Access

**Approach:**
```sql
-- Tickets module directly reads Events module's Venues table
SELECT Name, Capacity FROM events.venues WHERE id = @venueId;
```

**Pros:**
- Always up-to-date data
- No messaging infrastructure needed
- Simple to implement

**Cons:**
- **Violates module boundaries** - couples to database schema
- **Breaks encapsulation** - Tickets module bypasses Events module's logic
- **Schema coupling** - can't refactor Events database without breaking Tickets
- **No abstraction** - direct dependency on implementation details
- **Prevents future extraction** - can't move to microservices without major refactor
- **No audit trail** - can't see when/why data was accessed

**Why Rejected:**
This is the anti-pattern that modular monoliths are designed to avoid. Direct database access creates the tightest possible coupling - to the schema itself. The whole point of module boundaries and integration messages is to provide an abstraction layer that allows modules to evolve independently. Shared database access makes it impossible to refactor the Events module's database without coordinating with every consuming module.

---

### Alternative 4: Event-Carried State Transfer (Send Everything, Just in Case)

**Approach:**
```csharp
// Send all current and potential future properties
public record VenueUpserted
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public uint Capacity { get; init; }
    public AddressDto Address { get; init; }
    public string? PhoneNumber { get; init; }      // For future use
    public string? Website { get; init; }          // For future use
    public Dictionary<string, string> Metadata { get; init; }  // Just in case
}
```

**Pros:**
- Future-proof
- Reduces need for new messages
- Consumers can ignore what they don't need

**Cons:**
- Speculative - includes data for hypothetical requirements
- Larger messages
- Unclear what's actually needed
- Encourages lazy design (grab everything)
- Creates coupling to speculative features

**Why Rejected:**
YAGNI (You Aren't Gonna Need It). We should design for current requirements, not speculative future ones. If the Tickets module later needs additional venue information (e.g., phone number for customer support), we can add it to the message at that time. Over-engineering the message with unused data creates noise and coupling.

## Consequences

### Positive

1. **Loose Coupling** - Tickets module depends only on the data it actually uses
   - Events module can refactor Address value object without affecting Tickets
   - Internal implementation details (UK postcode validation) remain private
   - Clear contract: Tickets only cares about Id, Name, Capacity

2. **Module Autonomy** - Each module remains independent
   - Tickets module has its own Venue read model with only needed properties
   - Events module can evolve Venue aggregate without breaking consumers
   - Asynchronous messaging maintains availability decoupling

3. **Clear Responsibilities** - Data ownership is explicit
   - Events module owns Venue aggregate and its complete representation
   - Tickets module owns its read model with minimal venue data
   - No ambiguity about which module is source of truth

4. **Efficient Messaging** - Smaller message payloads
   - VenueUpserted is ~50 bytes (3 properties) vs ~150+ bytes (full aggregate)
   - Reduces RabbitMQ bandwidth and storage
   - Faster message serialization/deserialization

5. **Evolvable Design** - Can add properties later without breaking existing consumers
   - New properties added to VenueUpserted are optional
   - Existing consumers ignore unknown properties
   - Versioning is explicit (new fields are additive)

### Negative

1. **Potential for Additional Messages** - If requirements change, may need new messages
   - Example: If Tickets module later needs venue address, must update VenueUpserted
   - Requires coordination between modules (add to message, update consumer)
   - Migration: existing read models may be incomplete until re-synchronized

   **Mitigation**: Accept this as a reasonable trade-off. Adding a field is straightforward:
   - Add property to VenueUpserted record
   - Update publisher to include it
   - Update consumer to store it
   - Re-publish all venues to backfill

2. **Data Staleness** - Read model may be slightly out of sync
   - Eventual consistency between Events and Tickets modules
   - If venue capacity changes, Tickets module sees old value until message processed
   - Could theoretically allow over-booking in race condition

   **Mitigation**: This is inherent to event-driven architecture. In practice:
   - Message processing is fast (~milliseconds)
   - Capacity changes are infrequent
   - Tickets module validates against its snapshot (capacity check when reserving)
   - Acceptable trade-off for module autonomy

3. **No Cross-Module Validation** - Tickets module can't validate rules requiring excluded data
   - Example: Can't validate "no events in certain postcodes" because Address not included
   - Must choose: include data, or don't validate that rule

   **Mitigation**: Design decision - validation rules requiring data from other modules should be rare. If a rule requires data from another module, either:
   - Include that data in message (if genuinely needed)
   - Keep validation in owning module (better)

4. **Duplication of Core Identity Data** - Name and Capacity stored in both modules
   - Events module: Venue aggregate (source of truth)
   - Tickets module: Venue read model (cached copy)
   - Storage overhead (negligible: ~50 bytes per venue)

   **Mitigation**: This is intentional denormalization for read performance. The storage cost is trivial compared to benefits of module autonomy.

### Neutral

1. **Message Schema Evolution** - Changes to VenueUpserted affect all consumers
   - Breaking changes (removing fields) require coordination
   - Non-breaking changes (adding optional fields) are safe
   - Same as any event-driven system

2. **Consistency Model** - Eventual consistency is explicit
   - Not stronger or weaker than other patterns
   - Just different from synchronous queries

## Implementation Notes

### Applying This Pattern to New Integrations

When creating a new integration message, follow this process:

**1. Identify Consuming Module's Requirements:**
- What does the consuming module need to do?
- What data is required to fulfill those responsibilities?
- What data would be "nice to have" but not essential?

**2. Include Only Required Properties:**
- Identity (always include ID)
- Display data (if consumer shows it to users)
- Validation data (if consumer validates against it)
- Business logic data (if consumer makes decisions with it)

**3. Exclude Implementation Details:**
- Value objects used only by owning module
- Computed properties (consumer can compute if needed)
- Internal state that consumer doesn't use
- "Just in case" properties with no current use case

**4. Example Checklist (VenueUpserted):**
```
✅ Id          → Required (identity, foreign key)
✅ Name        → Required (display to users)
✅ Capacity    → Required (validate ticket quantity)
❌ Address     → Not needed (Tickets doesn't use it)
❌ CreatedAt   → Not needed (internal timestamp)
❌ UpdatedAt   → Not needed (internal timestamp)
```

### Real Example: VenueUpserted Message

**Publishing (Events Module):**
```csharp
// Infrastructure.Events/Venue/VenueRepository.cs
public void Add(Venue venue, out VenueUpserted message)
{
    dbContext.Venues.Add(venue);

    // Transform domain aggregate to integration message
    // Include ONLY what consumers need
    message = new VenueUpserted
    {
        Id = venue.Id,
        Name = venue.Name.ToString(),      // Value object → string
        Capacity = venue.Capacity
        // Address deliberately excluded
    };
}
```

**Consuming (Tickets Module):**
```csharp
// Messaging.Tickets/Consumers/VenueUpsertedConsumer.cs
public class VenueUpsertedConsumer : IConsumer<VenueUpserted>
{
    public async Task Consume(ConsumeContext<VenueUpserted> context)
    {
        var message = context.Message;

        // Upsert into Tickets module's read model
        await upsertVenue.Execute(
            message.Id,
            message.Name,
            message.Capacity
            // No Address - we don't have it and don't need it
        );
    }
}
```

**Read Model (Tickets Module):**
```csharp
// Domain.Tickets/Venue/Venue.cs
public class Venue : Entity
{
    public string Name { get; set; }
    public uint Capacity { get; set; }
    // Only stores what VenueUpserted provides (and what we need)
}
```

### When to Reconsider This Decision

**Add more data to message IF:**
- New business requirement in consuming module genuinely needs it
- Multiple messages would be needed without it (excessive chattiness)
- Data is stable and part of public contract

**DON'T add more data if:**
- "Might need it someday" (YAGNI)
- Can be derived or computed from existing data
- Implementation detail of owning module
- Creates coupling to internal structure

### Migration Path if Requirements Change

If Tickets module later needs venue Address:

```csharp
// 1. Add to VenueUpserted message
public record VenueUpserted
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public uint Capacity { get; init; }
    public AddressDto Address { get; init; }  // NEW PROPERTY
}

// 2. Update Tickets read model
public class Venue : Entity
{
    public string Name { get; set; }
    public uint Capacity { get; set; }
    public string? Address { get; set; }  // NEW PROPERTY (nullable for migration)
}

// 3. Update consumer
public async Task Consume(ConsumeContext<VenueUpserted> context)
{
    await upsertVenue.Execute(
        context.Message.Id,
        context.Message.Name,
        context.Message.Capacity,
        context.Message.Address  // Use new property
    );
}

// 4. Re-publish all venues to backfill Address in Tickets read model
// (One-time data migration)
```

This is straightforward and non-breaking for other consumers.

## Related Decisions

- **ADR-001: Initial Architecture** - Establishes modular monolith and async messaging pattern
  - This ADR provides specific guidance on WHAT to include in messages
  - Complements ADR-001's HIGH-LEVEL integration pattern

- **ADR-003: Vertical Slicing** - Describes code organization within modules
  - Integration messages live in `Messages.{Module}/` directory
  - Consumers live in `Messaging.{Module}/Consumers/`

- **Future**: May need ADR for message versioning strategy as system evolves
- **Future**: May need ADR for handling breaking changes in integration contracts

## References

- [Modular Monolith with DDD (Kamil Grzybek)](https://github.com/kgrzybek/modular-monolith-with-ddd)
- [Integration Patterns in Modular Monoliths](https://www.kamilgrzybek.com/blog/posts/modular-monolith-integration-patterns)
- [Event-Carried State Transfer (Martin Fowler)](https://martinfowler.com/articles/201701-event-driven.html)
- Implementation commits: `29aba8c` (cross-module venue synchronization)
- WIP.md session notes: 2026-01-03 Session 6

---

## Summary

**Decision**: Include **only minimal required data** in cross-module integration messages.

**Key Principle**: Send what consumers NEED, not what they MIGHT need or what's AVAILABLE.

**For VenueUpserted**: Send Id, Name, Capacity. Exclude Address (Tickets module doesn't use it).

**Rationale**: Minimizes coupling, preserves module autonomy, allows independent evolution.

**Result**: Events module can refactor Venue aggregate internals (Address value object, validation rules) without affecting Tickets module. Tickets module has everything it needs to fulfill its responsibilities.
