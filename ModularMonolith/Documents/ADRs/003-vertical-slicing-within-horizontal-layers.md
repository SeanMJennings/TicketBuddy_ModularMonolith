# ADR-003: Vertical Slicing Within Horizontal Layers

**Date**: 2025-12-31

**Tags**: architecture, organization, vertical-slices, modular-monolith, code-organization

**Supersedes**: Partially supersedes organizational aspects of ADR-001 (horizontal layering remains, but internal organization changes)

## Context

The TicketBuddy modular monolith initially organized code using **pure horizontal layering** as described in ADR-001:

```
Modules/Events/
├── Domain.Events/
│   ├── Entities/Event.cs
│   ├── Services/EventsValidator.cs
│   └── Contracts/IPersistEvents.cs
├── Application.Events/
│   ├── Commands/EventCommands.cs     # All commands grouped together
│   └── Queries/EventQueries.cs       # All queries grouped together
├── Infrastructure.Events/
│   └── Persistence/
│       ├── EventRepository.cs        # All persistence in one area
│       └── EventDbContext.cs
└── Controllers.Events/
    └── EventController.cs            # All endpoints in one controller
```

**Problems with pure horizontal layering:**

1. **Feature scatter** - A single feature (e.g., "Create Event") required changes across 4+ files in different directories
2. **Low cohesion** - Related code lived far apart; unrelated code lived together
3. **Difficult navigation** - Finding all code for a feature required traversing multiple directories
4. **Weak organization signal** - File structure didn't communicate what the module does (its behaviors)
5. **Large files** - `EventCommands.cs`, `EventQueries.cs`, and `EventController.cs` grew large with all module operations
6. **Merge conflicts** - Multiple developers working on different features touched the same files

**Key realization**: Horizontal layering (Domain → Application → Infrastructure → Controllers) is valuable for **enforcing dependency flow**, but doesn't help with **organizing around behavior**.

**Requirements for new organization:**
- Maintain hexagonal architecture boundaries (layers still enforce dependency rules)
- Organize code around behaviors/features for easier navigation
- Reduce file size by breaking up monolithic command/query classes
- Improve cohesion (related code lives together)
- Make codebase structure self-documenting

**Constraints:**
- Cannot break existing architectural tests (layer dependency rules must remain)
- Must preserve testability
- Should work with existing tooling (IDEs, build systems)

## Decision

We will organize code using **vertical slices within horizontal layers**.

**Hybrid approach:**
1. **Horizontal layers** (Domain, Application, Infrastructure, Controllers) remain as **architectural boundaries** enforcing dependency flow
2. **Within each layer**, code is organized by **behavior/feature** (vertical slices)

### New Structure

```
Modules/Events/
├── Domain.Events/
│   ├── Event.cs                      # Aggregate root at layer root
│   ├── EventsValidator.cs            # Domain services at layer root
│   ├── IPersistEvents.cs             # Contracts at layer root
│   └── IEventsUnitOfWork.cs
│
├── Application.Events/
│   ├── CreateEvent.cs                # One behavior = One file
│   ├── UpdateEvent.cs                # One behavior = One file
│   ├── GetEvents.cs                  # One behavior = One file
│   ├── GetEventById.cs               # One behavior = One file
│   └── MarkEventAsSoldOut.cs         # One behavior = One file
│
├── Infrastructure.Events/
│   ├── Core/                         # Shared infrastructure
│   │   ├── EventDbContext.cs
│   │   ├── UnitOfWork.cs
│   │   └── Configuration/
│   └── Event/                        # Entity-specific infrastructure
│       └── EventRepository.cs
│
└── Controllers.Events/
    ├── Event/                        # Grouped by entity/aggregate
    │   ├── CreateEventEndpoint.cs    # One endpoint = One file
    │   ├── UpdateEventEndpoint.cs    # One endpoint = One file
    │   ├── GetEventsEndpoint.cs      # One endpoint = One file
    │   └── GetEventByIdEndpoint.cs   # One endpoint = One file
    ├── Requests/                     # Shared DTOs
    └── Routes.cs
```

### Pattern: Behavior-First Organization

**Application Layer** - One class per behavior:
```csharp
// Application.Events/CreateEvent.cs
public class CreateEvent(EventsValidator validator, IPersistEvents repository, IEventsUnitOfWork unitOfWork)
{
    public async Task<Guid> Execute(EventName name, DateTimeOffset start, DateTimeOffset end, Venue venue, Money price)
    {
        // Complete behavior in one focused class
    }
}
```

**Controllers Layer** - One endpoint per file:
```csharp
// Controllers.Events/Event/CreateEventEndpoint.cs
[ApiController]
[Authorize(Roles = Roles.Admin)]
public class CreateEventEndpoint(CreateEvent createEvent) : ControllerBase
{
    [HttpPost(Routes.Events)]
    public async Task<CreatedResult> CreateEvent([FromBody] EventPayload payload)
    {
        var eventId = await createEvent.Execute(payload.EventName, payload.StartDate, payload.EndDate, payload.Venue, new Money(payload.Price));
        return Created($"/{Routes.Events}/{eventId}", eventId);
    }
}
```

### Pattern: Deeper Vertical Slicing for Complex Behaviors

For modules with more complexity (e.g., Tickets), vertical slices can go deeper:

```
Modules/Tickets/
├── Application.Tickets/
│   ├── Core/                         # Shared application utilities
│   ├── Ticket/                       # Organized by entity/aggregate
│   │   ├── ReserveTickets/           # Complex behavior gets folder
│   │   │   ├── ReserveTickets.cs
│   │   │   └── IExtendTicketsInTheReservationCache.cs
│   │   ├── PurchaseTickets/
│   │   │   ├── PurchaseTickets.cs
│   │   │   └── AllTicketsSoldHandler.cs  # Domain event handler for this behavior
│   │   ├── GetTicketsForEvent/
│   │   │   └── GetTicketsForEvent.cs
│   │   └── GetTicketsForUser/
│   │       └── GetTicketsForUser.cs
│   ├── Event/                        # Organized by related entity
│   │   └── UpsertEvent.cs            # Integration message handler
│   └── User/
│       └── UpsertUser.cs
│
├── Domain.Tickets/
│   ├── Core/                         # Shared domain logic
│   ├── Ticket/                       # Organized by aggregate
│   │   ├── Ticket.cs
│   │   ├── TicketsValidator.cs
│   │   ├── TicketsPurchaser.cs
│   │   ├── TicketsReleaser.cs
│   │   ├── IPersistTickets.cs
│   │   └── IQueryTickets.cs
│   ├── Event/                        # Read model for Event
│   │   └── IPersistEvents.cs
│   └── User/                         # Read model for User
│       └── IPersistUsers.cs
│
└── Infrastructure.Tickets/
    ├── Core/                         # Shared infrastructure
    │   ├── TicketDbContext.cs
    │   └── Configuration/
    ├── Ticket/                       # Ticket aggregate infrastructure
    │   ├── TicketRepository.cs
    │   ├── TicketQuerist.cs
    │   └── TicketReservationCacheRepository.cs
    ├── Event/                        # Event read model infrastructure
    │   └── EventRepository.cs
    └── User/                         # User read model infrastructure
        └── UserRepository.cs
```

**Principle: Depth of slicing proportional to complexity**
- Simple behaviors: Single file (e.g., `GetEvents.cs`)
- Complex behaviors with dependencies: Folder with related files (e.g., `ReserveTickets/`)
- Multiple aggregates: Organize by entity/aggregate first, then behavior

## Alternatives Considered

### Alternative 1: Pure Horizontal Layering (Status Quo Before This ADR)

**Structure:**
```
Application.Events/
├── Commands/
│   └── EventCommands.cs      # All commands in one class
└── Queries/
    └── EventQueries.cs       # All queries in one class
```

**Pros:**
- Traditional layered architecture pattern
- Clear separation of reads (queries) vs writes (commands)
- Familiar to developers from classic N-tier architecture

**Cons:**
- Large, monolithic files (`EventCommands.cs` contained 5+ methods)
- Low cohesion (unrelated commands grouped together)
- Feature scatter (one feature touched 4+ files across layers)
- Difficult to navigate (need to know which layer to look in)
- Merge conflicts when multiple developers work on different features
- File structure doesn't communicate what the module does

**Why Rejected**: The cons significantly outweigh the pros. While familiar, this pattern doesn't scale well as modules grow. The organization by technical concern (command vs query) rather than business behavior made the codebase harder to navigate and maintain.

---

### Alternative 2: Pure Vertical Slices (Eliminate Horizontal Layers)

**Structure:**
```
Modules/Events/
└── Features/
    ├── CreateEvent/
    │   ├── CreateEvent.cs            # Domain + Application + Infrastructure
    │   ├── CreateEventEndpoint.cs    # Controller
    │   └── CreateEventValidator.cs
    └── GetEvents/
        ├── GetEvents.cs
        └── GetEventsEndpoint.cs
```

**Pros:**
- Maximum cohesion (all code for a feature in one place)
- Easy navigation (everything for "Create Event" in one folder)
- Self-documenting structure
- Minimal file jumping when working on a feature
- Popular in some modern architectures (e.g., Jimmy Bogard's Vertical Slice Architecture)

**Cons:**
- **Loses architectural boundaries** - No enforcement of hexagonal architecture
- **Domain logic mixing with infrastructure** - Hard to test domain in isolation
- **Code duplication risk** - No clear place for shared domain services
- **Architecture tests become complex** - Harder to enforce dependency rules
- **Breaks with existing ADR-001** - Complete architectural pivot

**Why Rejected**: While this pattern maximizes cohesion for features, it sacrifices the architectural benefits of hexagonal architecture documented in ADR-001. We value the ability to test domain logic in isolation, enforce dependency flow, and maintain clear separation of concerns. The cognitive overhead of maintaining architectural discipline without structural enforcement is too high.

---

### Alternative 3: Namespace-Only Vertical Slicing (No Folders)

**Structure:**
```
Application.Events/
├── CreateEvent.cs          # namespace Application.Events.CreateEvent
├── UpdateEvent.cs          # namespace Application.Events.UpdateEvent
├── GetEvents.cs            # namespace Application.Events.GetEvents
└── GetEventById.cs         # namespace Application.Events.GetEventById
```

**Pros:**
- One behavior per file (solves large file problem)
- Simpler directory structure
- Less nesting
- Still maintains horizontal layers

**Cons:**
- All files in one directory (gets cluttered as module grows)
- No visual grouping by entity/aggregate
- Namespace-based organization less visible in IDEs
- Harder to find related behaviors (e.g., all Ticket operations)

**Why Rejected**: While simpler, this doesn't scale well for modules with many behaviors (e.g., Tickets module has 10+ behaviors). The flat structure makes it harder to understand the module's capabilities at a glance. Folders provide valuable visual organization.

---

### Alternative 4: Group by CQRS (Command/Query) First, Then Vertical Slice

**Structure:**
```
Application.Events/
├── Commands/
│   ├── CreateEvent/
│   │   └── CreateEvent.cs
│   └── UpdateEvent/
│       └── UpdateEvent.cs
└── Queries/
    ├── GetEvents/
    │   └── GetEvents.cs
    └── GetEventById/
        └── GetEventById.cs
```

**Pros:**
- Explicit CQRS separation
- Vertical slices within each side (command/query)
- Clear read vs write distinction

**Cons:**
- Extra nesting level (deeper directory structure)
- Separates related behaviors (e.g., CreateEvent and GetEventById live far apart)
- CQRS separation already implicit in method signatures (`Execute() → Guid` vs `Execute() → Event`)
- Adds complexity without clear benefit for our use case

**Why Rejected**: The CQRS distinction is valuable but doesn't need to be encoded in the folder structure. The method signatures already make this clear (`Task<Guid> Execute(...)` = command, `Task<Event> Execute(...)` = query). The extra nesting level reduces cohesion by separating related operations (e.g., creating and retrieving events).

## Consequences

### Positive

1. **High cohesion** - All code for a behavior lives in the same area
   - Example: `CreateEvent.cs` contains the entire "create event" behavior
   - Related behaviors grouped by entity (all Ticket operations under `Ticket/`)

2. **Self-documenting structure** - Directory structure communicates module capabilities
   - Looking at `Application.Events/` immediately shows what the module does
   - Folder names use ubiquitous language from the domain

3. **Easier navigation** - Finding code for a feature requires less jumping
   - Work on "Reserve Tickets"? → `Application.Tickets/Ticket/ReserveTickets/`
   - All related files in one place (behavior + its dependencies)

4. **Smaller files** - One class per file, focused on single behavior
   - Before: `EventCommands.cs` with 100+ lines, 5+ methods
   - After: `CreateEvent.cs` with ~20 lines, 1 method

5. **Reduced merge conflicts** - Different features touch different files
   - Developer A works on `CreateEvent.cs`
   - Developer B works on `UpdateEvent.cs`
   - No conflicts (previously both touched `EventCommands.cs`)

6. **Maintains architectural benefits** - Horizontal layers still enforce hexagonal architecture
   - Architecture tests still pass (enforce Domain → Application → Infrastructure)
   - Domain remains pure (no infrastructure dependencies)
   - Easy to test domain logic in isolation

7. **Scalable organization** - Pattern scales from simple to complex modules
   - Events module: Simple (flat files per behavior)
   - Tickets module: Complex (folders for behaviors with multiple files)

8. **Better code review** - PRs show clear feature scope
   - Files changed: `CreateEvent.cs`, `CreateEventEndpoint.cs`, `Event.cs`
   - Immediately clear what feature is being added

### Negative

1. **More files** - One class per file increases file count
   - Before: 2 files (`EventCommands.cs`, `EventQueries.cs`)
   - After: 5+ files (one per behavior)
   - Mitigation: Better organization and smaller files outweigh this cost

2. **Deeper nesting** - Complex behaviors create nested folders
   - Example: `Application.Tickets/Ticket/ReserveTickets/ReserveTickets.cs`
   - 4 levels deep (module → layer → entity → behavior)
   - Mitigation: Only apply deep nesting when behavior complexity justifies it

3. **Duplication of folder names** - Some folders and files share names
   - `Application.Tickets/Ticket/ReserveTickets/ReserveTickets.cs`
   - Folder `ReserveTickets` contains class `ReserveTickets`
   - Mitigation: This is intentional; folder groups related files, class implements behavior

4. **Learning curve** - New pattern requires explanation
   - Developers familiar with classic layering need to learn hybrid approach
   - Need to document when to use file vs folder for a behavior
   - Mitigation: This ADR + updated architecture guide provide guidance

5. **Inconsistency between modules** - Simple vs complex modules look different
   - Events: Flat files (`CreateEvent.cs`)
   - Tickets: Nested folders (`Ticket/ReserveTickets/`)
   - Mitigation: Inconsistency reflects actual complexity difference; pattern is consistent

### Neutral

1. **Architecture tests unchanged** - Layer dependency rules still enforced
   - Domain cannot reference Application
   - Application cannot reference Infrastructure
   - Tests verify at assembly level (not affected by internal folders)

2. **IDE navigation slightly different** - Navigate by folder vs class name
   - Before: Find `EventCommands` class → find method
   - After: Find `CreateEvent` folder/file
   - Different, not worse (arguably better for discoverability)

3. **Namespace strategy varies** - Flexibility in namespace depth
   - Can use flat: `namespace Application.Events;`
   - Or nested: `namespace Application.Events.CreateEvent;`
   - Current approach uses flat for simplicity

## Implementation Notes

### Guidelines for Organizing Code

**When to use a file (simple behavior):**
- Single responsibility behavior
- No additional dependencies beyond common contracts
- Example: `GetEvents.cs`, `CreateEvent.cs`

**When to use a folder (complex behavior):**
- Behavior requires additional interfaces/dependencies
- Domain event handlers specific to this behavior
- Multiple supporting classes
- Example: `ReserveTickets/` (contains `ReserveTickets.cs` + `IExtendTicketsInTheReservationCache.cs`)

**Organizing by entity/aggregate:**
- If module has multiple aggregates, organize first by aggregate
- Example: `Ticket/`, `Event/`, `User/` folders
- Keeps related behaviors together

**Shared code:**
- Core/shared utilities go in `Core/` folder at layer root
- Example: `Infrastructure.Tickets/Core/TicketDbContext.cs`
- Entity-specific code goes in entity folder

### Migration Path

This ADR documents a refactor completed in commits:
- `2af74b4` - Initial refactor towards vertical slices
- `1d28898` - Refactor into vertical slices (main refactor)
- `0453db8` - Vertically slice controllers
- `549d34f` - Final cleanup

**Refactor process used:**
1. Started with Application layer (broke up `EventCommands.cs` into individual files)
2. Moved to Controllers layer (broke up `EventController.cs` into endpoint files)
3. Organized Infrastructure by entity/aggregate
4. Created `Core/` folders for shared infrastructure
5. Updated tests to reference new file locations
6. Verified architecture tests still pass

**Result:**
- **69 files changed**
- **173 insertions, 747 deletions** (net reduction of 574 lines!)
- All tests passing
- Architecture boundaries maintained

### Testing Implications

**No changes to test structure:**
- Tests still organized by layer (`Testing.Unit.Events`, `Testing.Integration.Events`)
- BDD specs and steps pattern unchanged
- Architecture tests verify layer dependencies (unaffected by internal organization)

**Easier to test:**
- Smaller, focused classes easier to unit test
- Clear behavior → test mapping (test `CreateEvent` behavior, test `CreateEvent` class)
- Reduced setup complexity (fewer dependencies per class)

## Related Decisions

- **ADR-001: Initial Architecture** - Establishes horizontal layers (Domain, Application, Infrastructure, Controllers)
  - This ADR keeps those layers as architectural boundaries
  - Changes internal organization within each layer

- **ADR-002: Initial Testing** - Establishes BDD testing pattern
  - Testing structure unchanged
  - Smaller classes make tests simpler

- **Future**: May need ADR for folder naming conventions as codebase grows
- **Future**: May need ADR for when to split behaviors into separate modules

## References

- [Vertical Slice Architecture (Jimmy Bogard)](https://www.jimmybogard.com/vertical-slice-architecture/)
- [Feature Folders vs Technical Folders](https://www.youtube.com/watch?v=PRns0rqPonA)
- Git refactor commits: `1d28898`, `0453db8`, `549d34f`
- [Organizing Code by Feature (CodeOpinion)](https://codeopinion.com/organizing-code-by-feature-using-vertical-slices/)

---

## Summary

**Decision**: Organize code using **vertical slices within horizontal layers** - a hybrid approach that combines the architectural benefits of hexagonal architecture (enforced via horizontal layers) with the navigational and cohesion benefits of feature-based organization (vertical slices).

**Key Principle**: **Horizontal layers enforce architecture; vertical slices organize behavior.**

**Result**: More cohesive, self-documenting, and maintainable codebase that preserves architectural boundaries while organizing around business behaviors rather than technical concerns.