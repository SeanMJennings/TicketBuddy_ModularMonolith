# WIP: Venue Aggregate in Events Module

**Started**: 2025-12-31
**Status**: In Progress
**Current Step**: 5 (GetVenues Complete)

## Goal

Transform Venue from a hardcoded enum (`Domain.ValueObjects.Venue`) into a proper aggregate in the Events module, enabling dynamic venue management and cross-module communication to the Tickets module.

**Business Value**: Venues can be created/managed at runtime instead of being hardcoded.

## Current State Analysis

**Current Implementation:**
- `Domain.ValueObjects.Venue` is an enum with 10 hardcoded venues
- Events module uses this enum in `Event` aggregate
- Tickets module has `Venue` entity (read model: Id, Name, Capacity)
- Cross-module pattern exists: `EventUpserted` message (Events → Tickets)

**Testing Pattern:**
- Unit tests: Domain layer only (aggregates, which test underlying VOs)
- Integration tests: Controllers layer (full stack)
- BDD style: `.specs.cs` + `.steps.cs` files

**Value Object Pattern:**
- `readonly struct` (not class/record)
- Uses `StringValueObject<T>` for string-based VOs
- Uses `Validation.BasedOn()` for validation

## Overall Plan

### Phase 1: Domain Model - Unit Tests (Steps 1-3) ✅
1. ~~Create Venue aggregate test (tests Venue + Address + VenueName together)~~ - TDD ✅
2. ~~Create VenuesValidator domain service~~ - TDD ✅
3. ~~Create IPersistVenues port~~ - TDD ✅

### Phase 2: Venue Management - Integration Tests (Steps 4-6) ✅
4. ~~Create CreateVenue (behavior + endpoint + repository)~~ - TDD ✅
5. ~~Create GetVenues (behavior + endpoint + repository)~~ - TDD ✅
6. ~~Create GetVenueById (behavior + endpoint + repository)~~ - TDD ✅ (implemented with step 4)

### Phase 3: Update Event to Use VenueId (Steps 7-8)
7. Update Event aggregate to use VenueId (Guid) - TDD (unit test)
8. Update CreateEvent (behavior + endpoint) - TDD (integration test)

### Phase 4: Cross-Module Communication (Steps 9-11)
9. Create VenueUpserted message - TDD
10. Publish VenueUpserted when venue created - TDD (integration test)
11. Tickets: VenueUpsertedConsumer + UpsertVenue - TDD (integration test)

### Phase 5: Migration & Cleanup (Steps 12-14)
12. Create database migration for Venues table
13. Seed initial venues from enum (with UK addresses)
14. Remove Venue enum (breaking change)

## Current Focus

**Phase 2 Complete**: Venue Management Endpoints (COMPLETE)

**Next Action**: Step 7 - Update Event aggregate to use VenueId

**Tests Passing**: 107/107 tests passing (24 unit + 20 architecture + 22 integration + 11 component + 1 acceptance + 29 other)

## Agent Checkpoints

- [x] tdd-guardian: Steps 1-3 verified (RED-GREEN-REFACTOR followed)
- [ ] tdd-guardian: Verify TDD for remaining steps (11 more)
- [ ] refactor-scan: After each TDD session + at end of feature
- [ ] adr: If needed for architectural decision
- [ ] learn: Document patterns (AsyncSpecification, Address VO, cross-module messaging)
- [ ] docs-guardian: Update README when complete

## Architectural Decisions

**1. Venue as Aggregate Root**
- Independent lifecycle
- Identified by unique Address

**2. Address Uniqueness**
- Business rule: No two venues at same address
- Two venues CAN have same name at different addresses
- Enforced by VenuesValidator

**3. Address Value Object (UK Only)**
- Components: Street, City, Postcode
- UK postcode validation
- Events module only

**4. Capacity Constraints**
- Business rule: Venues limited to maximum 50 seats
- Minimum 1 seat (cannot be zero)
- Enforced by Venue aggregate validation

**5. Cross-Module Minimization**
- Tickets only needs: Id, Name, Capacity
- Address NOT sent to Tickets

**6. Testing Strategy**
- Unit test: Venue.specs.cs tests Venue aggregate (includes Address, VenueName validation)
- Integration tests: VenueController.specs.cs (full stack)

## Domain Model Design

**Venue Aggregate:**
```csharp
// Domain.Events/Venue/Venue.cs
public class Venue : Entity, IAmAnAggregateRoot
{
    private const uint MinCapacity = 1;
    private const uint MaxCapacity = 50;
    
    public Venue(Guid id, VenueName name, Address address, uint capacity) : base(id)
    {
        Validation.BasedOn(errors =>
        {
            if (capacity < MinCapacity) 
                errors.Add("Capacity must be at least 1");
            if (capacity > MaxCapacity) 
                errors.Add("Capacity cannot exceed 50 seats");
        });
        
        Name = name;
        Address = address;
        Capacity = capacity;
    }
    
    public VenueName Name { get; private set; }
    public Address Address { get; private set; }
    public uint Capacity { get; private set; }
}
```

## Vertical Slicing (ADR-003)

```
Domain.Events/Venue/
├── Venue.cs                 - Aggregate
├── VenueName.cs             - Value object
├── Address.cs               - Value object
├── VenuesValidator.cs       - Domain service
└── IPersistVenues.cs        - Port

Application.Events/Venue/
├── CreateVenue.cs
├── GetVenues.cs
└── GetVenueById.cs

Infrastructure.Events/Venue/
└── VenueRepository.cs

Controllers.Events/Venue/
├── CreateVenueEndpoint.cs
├── GetVenuesEndpoint.cs
└── GetVenueByIdEndpoint.cs

Controllers.Events/Requests/
└── VenuePayload.cs

Messages.Events/
└── VenueUpserted.cs
```

## Testing Structure

**Unit Test Scenarios:**
- Creating venue with valid data (1-50 seats)
- VenueName: null/empty name
- Address: null/empty street
- Address: null/empty city
- Address: invalid UK postcode
- Capacity: zero capacity (min violation)
- Capacity: 51+ seats (max violation)
- Address equality
- VenueName equality

**Integration Test Scenarios:**
- Creating venue via API
- Getting all venues
- Getting venue by ID
- Address uniqueness validation
- Publishing VenueUpserted message

## Next Steps

1. **RED**: Write failing test for Venue creation
2. Create Venue.specs.cs and Venue.steps.cs
3. **GREEN**: Implement Venue + Address + VenueName
4. **REFACTOR**: Assess improvements
5. **COMMIT**: Commit domain model

## Blockers

None

## Technical Notes

**Capacity Constraints:**
- Minimum: 1 seat
- Maximum: 50 seats
- Enforced by Venue aggregate

**UK Postcode Pattern:**
- ^[A-Z]{1,2}[0-9]{1,2}[A-Z]?\s?[0-9][A-Z]{2}$

**VenueUpserted Message:**
```csharp
public record VenueUpserted
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public uint Capacity { get; init; }
}
```

## Session Log

### 2025-12-31 - Session 1 (Planning)
**Completed**:
- Investigated Venue enum
- Analyzed value object patterns
- Analyzed testing patterns
- Created 14-step plan
- Defined capacity constraints (1-50 seats)

**Learned**:
- Value objects: readonly struct
- StringValueObject<T> pattern
- Validation.BasedOn()
- Single unit test file tests aggregate + VOs
- Integration tests cover full stack
- Address uniqueness (not name)
- Tickets needs: Id, Name, Capacity
- Capacity: 1-50 seats

**Next Session**:
- Step 2: Create VenuesValidator domain service
- Step 3: Create IPersistVenues port

**Agent Actions**:
- wip-guardian: Created WIP.md

### 2025-12-31 - Session 2 (Step 1 Complete)
**Duration**: ~30 minutes
**Completed**:
- Step 1: Created Venue aggregate with Address and VenueName value objects
- RED: 7 failing unit tests (compilation errors)
- GREEN: Implemented Address, VenueName, Venue
- REFACTOR: Assessed - no changes needed
- Fixed namespace conflict with existing Venue enum
- Committed: bfff74a

**Learned**:
- Commit messages should be one-liners (no multi-paragraph format)
- Do not include "Generated with Claude Code" in commits
- readonly struct pattern for value objects works well
- UK postcode regex: ^[A-Z]{1,2}[0-9]{1,2}[A-Z]?\s?[0-9][A-Z]{2}$
- Namespace conflicts resolved with fully qualified names

**Next Session**:
- Step 2: VenuesValidator (address uniqueness check)
- Step 3: IPersistVenues port

**Agent Actions**:
- tdd-guardian: Verified RED-GREEN-REFACTOR cycle
- refactor-scan: No refactoring needed
- wip-guardian: Updated WIP.md

### 2026-01-01 - Session 3 (Steps 2-3 Complete)
**Duration**: ~45 minutes
**Completed**:
- Step 2: Created VenuesValidator domain service
- Step 3: Created IPersistVenues port
- RED: Test for address uniqueness validation (compilation errors, then logic failure)
- GREEN: Implemented VenuesValidator.CheckAddressUniqueness and IPersistVenues.GetAll
- REFACTOR: Assessed - no changes needed
- Committed: e1911b2

**Learned**:
- AsyncSpecification pattern for async tests
- await When(Validating(async_method)) pattern for async validation tests
- before_each() must return Task when inheriting AsyncSpecification
- Testing async domain services with mocks

**Next Session**:
- Step 4: CreateVenue (behavior + endpoint + repository) - integration test
- Will need to set up testcontainers for database
- VenueRepository will implement IPersistVenues

**Agent Actions**:
- tdd-guardian: Verified RED-GREEN-REFACTOR cycle for steps 2-3
- refactor-scan: No refactoring needed
- wip-guardian: Updated WIP.md

### 2026-01-01 - Session 4 (Step 4 Complete)
**Duration**: ~90 minutes
**Completed**:
- Step 4: Created CreateVenue full vertical slice (integration test)
- Step 6: Created GetVenueById (implemented alongside step 4)
- RED: Integration test failing (NullReferenceException, then DI errors, then EF Core binding errors, then missing table)
- GREEN: Implemented full stack - behavior, endpoints, repository, DB context, migration
- REFACTOR: Assessed - no changes needed
- Fixed 3 major issues: DI registration, EF Core constructor binding, database migration

**Learned**:
- Integration test patterns with Testcontainers
- Dictionary<Type,Type> required for DomainEventsMapper DI
- EF Core ComplexProperty configuration for value type structs
- EF Core requires parameterless constructor for entity reconstruction
- Namespace collision handling with explicit qualification (Domain.ValueObjects.Venue)
- Database migration required for ExcludeFromMigrations tables
- IPersistVenues.Add changed from Task to void (synchronous)

**Next Session**:
- Step 5: GetVenues (behavior + endpoint) - integration test
- Should be straightforward following CreateVenue pattern

**Agent Actions**:
- wip-guardian: Fixed DI, EF Core, and migration issues
- wip-guardian: Updated WIP.md

**Files Created** (11):
- VenueController.specs.cs, VenueController.steps.cs
- CreateVenue.cs, GetVenueById.cs
- CreateVenueEndpoint.cs, GetVenueByIdEndpoint.cs
- VenuePayload.cs, VenueRepository.cs
- 013-CreateVenuesTable.sql

**Files Modified** (6):
- IPersistVenues.cs, EventDbContext.cs, Routes.cs
- Services.cs, EventPayload.cs, CreateEvent.cs

### 2026-01-01 - Session 5 (Step 5 Complete)
**Duration**: ~20 minutes
**Completed**:
- Step 5: Created GetVenues endpoint (integration test)
- RED: Test failing (compilation error - GetVenuesEndpoint doesn't exist)
- GREEN: Implemented GetVenues behavior and endpoint
- REFACTOR: Assessed - no changes needed
- Phase 2 complete: All venue management endpoints implemented

**Learned**:
- Simple endpoint implementation following established patterns
- Test data setup patterns for multiple entities
- Repository.GetAll() already existed from domain model setup
- Integration tests build on previous infrastructure

**Next Session**:
- Step 7: Update Event aggregate to use VenueId (Guid) instead of Venue enum
- Will need unit tests for Event aggregate changes
- Breaking change to Event domain model

**Agent Actions**:
- None required - straightforward implementation

**Files Created** (2):
- GetVenues.cs
- GetVenuesEndpoint.cs

**Files Modified** (3):
- VenueController.specs.cs (added can_list_venues test)
- VenueController.steps.cs (added test steps)
- Services.cs (registered GetVenues)
