# WIP: UpdateVenue Feature Implementation

**Started**: 2026-01-04
**Status**: In Progress
**Current Step**: 6 of 13

## Goal

Implement UpdateVenue endpoint (PUT /venues/{id}) for the Events module, following the same pattern as UpdateEvent. This enables administrators to update venue details while maintaining cross-module synchronization with the Tickets module via VenueUpserted message.

## Overall Plan

### Phase 1: Domain Layer (Steps 1-3)
1. Add Update methods to Venue aggregate
2. Add CheckVenueExists to VenuesValidator
3. Add CheckAddressUniqueness with exclusion logic

### Phase 2: Application Layer (Steps 4-5)
4. Implement UpdateVenue use case
5. Add Update method to IPersistVenues and VenueRepository

### Phase 3: Controller Layer (Step 6)
6. Create UpdateVenueEndpoint with UpdateVenuePayload

### Phase 4: Integration Tests (Steps 7-9)
7. Integration test: Update venue successfully
8. Integration test: Address uniqueness validation on update
9. Integration test: VenueUpserted message published on update

### Phase 5: Component Tests (Steps 10-12)
10. Component test: Update venue via API
11. Component test: Verify address uniqueness across modules
12. Component test: Cross-module sync on update

### Phase 6: Completion (Step 13)
13. Final verification and documentation

## Current Focus

**Step 6**: Create UpdateVenueEndpoint (Controller layer)

**Status**: Not started
**Tests Passing**: 119/119
**Last Commit**: feat: implement UpdateVenue use case and repository method

**Plan for this step:**
1. Read UpdateEventEndpoint.cs to understand the pattern
2. Create UpdateVenuePayload request record
3. Create UpdateVenueEndpoint with PUT /venues/{id}
4. Build and verify compilation
5. Commit with message: "feat: add UpdateVenue endpoint"

**Expected Changes:**
- File: `Modules\Events\Controllers.Events\Venue\UpdateVenueEndpoint.cs` (CREATE)
  - PUT endpoint at /venues/{id}
  - Admin-only authorization
  - Call UpdateVenue use case
  - Return NoContent on success
- File: `Modules\Events\Controllers.Events\Requests\UpdateVenuePayload.cs` (CREATE)
  - Request payload with Name, Street, City, Postcode, Capacity

## Completed Steps

**Step 1**: Add Update methods to Venue aggregate ✓
- Added UpdateName(), UpdateAddress(), UpdateCapacity() methods to Venue aggregate
- Implemented capacity validation (1-50 range)
- Extracted ValidateCapacity() private method to eliminate duplication
- Tests: 116/116 passing
- Commits:
  - refactor: extract capacity validation to private method
  - feat: add update methods to venue aggregate

**Step 2**: Add CheckVenueExists to VenuesValidator ✓
- Added CheckVenueExists(Guid venueId) method to VenuesValidator
- Retrieves venue by ID or throws ValidationException if not found
- Added 1 unit test (happy path only, error case tested at integration level)
- Tests: 117/117 passing
- Commit:
  - feat: add CheckVenueExists validator method

**Step 3**: Add CheckAddressUniqueness with exclusion logic ✓
- Modified CheckAddressUniqueness to accept optional excludeVenueId parameter
- Filters out excluded venue from uniqueness check using LINQ Where clause
- Added 2 unit tests (can keep same address, cannot use another venue's address)
- Tests: 119/119 passing
- Commit:
  - feat: add address exclusion to uniqueness check

**Step 4**: Implement UpdateVenue use case ✓
- Created UpdateVenue.cs orchestrating validation and persistence
- Calls CheckVenueExists, applies updates, validates address uniqueness, persists
- Follows UpdateEvent pattern with dependency injection
- Tests: 119/119 passing

**Step 5**: Add Update method to repository ✓
- Added Update(Venue venue) signature to IPersistVenues interface
- Implemented Update method in VenueRepository
- Publishes VenueUpserted message on update (Id, Name, Capacity)
- Tests: 119/119 passing
- Commit (combined 4-5):
  - feat: implement UpdateVenue use case and repository method

## Agent Checkpoints

- [ ] Step 1: tdd-guardian - Verify TDD compliance for domain update methods
- [ ] Step 2: tdd-guardian - Verify TDD compliance for VenuesValidator.CheckVenueExists
- [ ] Step 3: tdd-guardian - Verify TDD compliance for address uniqueness with exclusion
- [ ] Step 4: tdd-guardian - Verify TDD compliance for UpdateVenue use case
- [ ] Step 5: tdd-guardian - Verify TDD compliance for repository Update method
- [ ] Step 6: tdd-guardian - Verify TDD compliance for UpdateVenueEndpoint
- [ ] Step 7-9: tdd-guardian - Verify integration tests follow behavior-driven approach
- [ ] Step 10-12: tdd-guardian - Verify component tests cover cross-module scenarios
- [ ] After each GREEN: refactor-scan - Assess improvement opportunities
- [ ] Step 13: docs-guardian - Update README if needed (likely minimal changes)
- [ ] Step 13: learn - Document any gotchas or patterns discovered

## Next Steps

1. Write unit test for CheckAddressUniqueness with same address (allowed when excluding current venue)
2. Write unit test for CheckAddressUniqueness with conflicting address (should throw even with exclusion)
3. Modify CheckAddressUniqueness to support optional excludeVenueId parameter
4. Assess refactoring opportunities
5. Begin Step 4: Implement UpdateVenue use case

## Blockers

None currently

## Technical Notes

### Reference Pattern: UpdateEvent
The UpdateEvent feature provides the implementation pattern:
- **Domain**: Event aggregate has UpdateName(), UpdateDates(), UpdatePrice() methods
- **Application**: UpdateEvent use case orchestrates validation and persistence
- **Validator**: EventsValidator.CheckEventExists() retrieves and validates entity
- **Repository**: EventRepository.Update() persists changes (not yet seen - need to check)
- **Controller**: UpdateEventEndpoint receives payload, calls use case, returns NoContent
- **Authorization**: Admin-only via `[Authorize(Roles = Roles.Admin)]`

### Key Differences for Venue
1. **Address Uniqueness**: Must check address hasn't changed to another existing venue's address
   - Challenge: Need to exclude current venue from uniqueness check
   - Solution: Add optional `excludeVenueId` parameter to CheckAddressUniqueness
2. **VenueUpserted Message**: Must publish on update (not just create)
   - Current: VenueRepository.Add() publishes VenueUpserted
   - Needed: VenueRepository.Update() must also publish VenueUpserted
   - Message contains: Id, Name, Capacity (Address NOT included per ADR-004)

### Files to Create/Modify

**Domain Layer:**
- MODIFY: `Modules\Events\Domain.Events\Venue\Venue.cs`
  - Add UpdateName, UpdateAddress, UpdateCapacity methods
- MODIFY: `Modules\Events\Domain.Events\Venue\VenuesValidator.cs`
  - Add CheckVenueExists method
  - Modify CheckAddressUniqueness to support exclusion
- MODIFY: `Modules\Events\Testing.Unit.Events\Venue.steps.cs`
  - Add test steps for update operations

**Application Layer:**
- CREATE: `Modules\Events\Application.Events\Venue\UpdateVenue.cs`
  - Use case orchestrating update operation
- MODIFY: `Modules\Events\Domain.Events\Venue\IPersistVenues.cs`
  - Add Update method signature
- MODIFY: `Modules\Events\Infrastructure.Events\Venue\VenueRepository.cs`
  - Implement Update method with VenueUpserted publishing

**Controller Layer:**
- CREATE: `Modules\Events\Controllers.Events\Venue\UpdateVenueEndpoint.cs`
  - PUT endpoint at /venues/{id}
- CREATE: `Modules\Events\Controllers.Events\Requests\UpdateVenuePayload.cs`
  - Request payload record

**Infrastructure/Configuration:**
- MODIFY: `Modules\Events\Infrastructure.Events\Core\Configuration\Services.cs`
  - Register UpdateVenue use case

**Integration Tests:**
- MODIFY: `Modules\Events\Testing.Integration.Events\VenueController.steps.cs`
  - Add steps for update scenarios
  - Add steps for address uniqueness validation
  - Add steps for message publishing verification
- MODIFY: `Modules\Events\Testing.Integration.Events\VenueController.specs.cs`
  - Add test specifications for update scenarios

**Component Tests:**
- MODIFY: `Testing\Testing.Component\Api\VenueApi.steps.cs`
  - Add steps for update via API
  - Add steps for cross-module sync verification
- MODIFY: `Testing\Testing.Component\Api\VenueApi.specs.cs`
  - Add specifications for update scenarios

### Potential Gotchas

1. **Address Uniqueness Validation**:
   - When updating, must allow keeping the same address
   - Must prevent changing to an address that belongs to a different venue
   - Solution: Exclude current venue ID from uniqueness check

2. **VenueUpserted Publishing**:
   - Currently only published in Add() method
   - Must also publish in Update() method
   - Message structure: `{ Id: Guid, Name: string, Capacity: uint }`
   - Address NOT included (per ADR-004 cross-module data minimization)

3. **Repository Update Pattern**:
   - EventRepository.Update() pattern needs to be verified
   - EF Core change tracking may handle update automatically
   - Need to ensure VenueUpserted is published

4. **Capacity Validation**:
   - Must enforce MinCapacity (1) and MaxCapacity (50) constraints
   - Same validation as constructor
   - Consider extracting to shared validation method

5. **Integration Test Setup**:
   - VenueController.steps.cs uses testHarness for message verification
   - Need UpdateVenueEndpoint in service provider configuration
   - Pattern established in EventController.steps.cs

6. **Cross-Module Impact**:
   - Tickets module consumes VenueUpserted via VenueUpsertedConsumer
   - UpsertVenue in Tickets module handles both create and update
   - Update message will trigger venue data sync in Tickets module
   - Component tests should verify this cross-module behavior

### Test Strategy

**Unit Tests (Domain Layer):**
- Test each update method independently
- Test capacity validation in UpdateCapacity
- Test VenuesValidator.CheckVenueExists throws when venue not found
- Test address uniqueness with and without exclusion

**Integration Tests (Module Level):**
- Test successful venue update
- Test 404 when venue doesn't exist
- Test validation error when address conflicts with another venue
- Test VenueUpserted message is published
- Test admin authorization requirement

**Component Tests (Cross-Module):**
- Test update via HTTP API
- Test cross-module sync (Tickets module receives update)
- Test address uniqueness across actual database

### TDD Workflow per Step

Each step follows strict RED-GREEN-REFACTOR:

**RED Phase:**
- Write test describing desired behavior
- Run test, verify it fails for the right reason
- Commit: "test: [description of behavior being tested]"

**GREEN Phase:**
- Write MINIMUM code to make test pass
- Run test, verify it passes
- Run ALL tests, verify nothing broken
- Commit: "feat: [description of feature implemented]"

**REFACTOR Phase:**
- Invoke refactor-scan agent
- If improvements identified and valuable, refactor
- Run ALL tests, verify still passing
- Commit: "refactor: [description of improvement]"

## Session Log

### 2026-01-04 - Session 1
**Duration**: ~90 minutes
**Completed**:
- WIP document created
- **Phase 1 (Domain Layer) - COMPLETE**
  - Step 1: Add Update methods to Venue aggregate
    - Added 5 unit tests for UpdateName, UpdateAddress, UpdateCapacity (including validation)
    - Implemented update methods on Venue aggregate
    - Refactored capacity validation to private method
    - All 116 tests passing
    - 2 commits made
  - Step 2: Add CheckVenueExists to VenuesValidator
    - Added CheckVenueExists(Guid venueId) method
    - Added 1 unit test (happy path)
    - All 117 tests passing
    - 1 commit made
  - Step 3: Add CheckAddressUniqueness with exclusion logic
    - Modified CheckAddressUniqueness to support optional excludeVenueId
    - Added 2 unit tests for exclusion scenarios
    - All 119 tests passing
    - 1 commit made
**Next**: Step 4 - Implement UpdateVenue use case (Application layer)
