# WIP: Venues Entity - Replace Hardcoded Enum with API-Backed Entity

**Started**: 2026-01-05
**Status**: Not Started
**Current Step**: 0 of 28

## Goal

Replace the hardcoded `Venue` enum with a full API-backed entity system. Venues will be managed through CRUD endpoints, with admin controls for creation/updates and public access for reading. Events will reference venues by ID rather than enum value.

## Overall Plan

### Phase 0: Prerequisites
0. Install Zod for schema validation at API boundaries

### Phase 1: Domain & API Layer (Steps 1-5)
1. Create venue domain types (Venue, VenuePayload, Address)
2. Create venue API client (GET /venues, GET /venues/:id)
3. Add venue creation API (POST /venues - admin only)
4. Add venue update API (PUT /venues/:id - admin only)
5. Add venue list API integration

### Phase 2: Venues Management UI (Steps 6-12)
6. Create VenuesManagement component skeleton
7. Implement venue list display
8. Implement create venue form
9. Implement update venue form
10. Add admin-only routing/access control
11. Add venue management to navigation
12. End-to-end venue management tests

### Phase 3: Update Event Domain (Steps 13-16)
13. Update Event type to use venueId instead of Venue enum
14. Update EventPayload to use venueId
15. Update event API client to send/receive venueId
16. Verify event API integration with venue IDs

### Phase 4: Update Event UI - VenueDisplay Component (Steps 17-19)
17. Create VenueDisplay component (fetches venue by ID, displays name/address)
18. Test VenueDisplay with loading/error states
19. Verify VenueDisplay component behavior

### Phase 5: Integrate VenueDisplay Across App (Steps 20-24)
20. Update EventsManagement to use VenueDisplay
21. Update Home to use VenueDisplay
22. Update TicketPurchase to use VenueDisplay
23. Update UserProfile to use VenueDisplay
24. Verify all venue displays work correctly

### Phase 6: Cleanup Legacy Code (Steps 25-28)
25. Remove Venue enum from event.ts
26. Remove ConvertVenueToString helper
27. Remove all imports of Venue enum and helper
28. Final verification - all tests passing

## Current Focus

**Phase 1 Complete**: Domain & API Layer done

**Status**: Phase 2 ready to begin
**Tests Passing**: 66/66
**Last PR**: N/A

## Agent Checkpoints

- [ ] tdd-guardian: Verify TDD compliance for each step (28 times)
- [ ] ts-enforcer: Validate types (no `any`, schema-first at boundaries)
- [ ] refactor-scan: After each GREEN phase
- [ ] adr: Create ADRs for architectural decisions (as they arise)
- [ ] learn: Document learnings in CLAUDE.md
- [ ] docs-guardian: Update permanent docs when feature completes

## Next Steps

1. Start Phase 2, Step 6: Create VenuesManagement component skeleton
2. Implement venue list display
3. Implement create/update venue forms
4. Add admin routing and navigation

## Blockers

None currently

## Technical Notes

**Current State:**
- Venues hardcoded as enum in `src/domain/event.ts`
- `ConvertVenueToString()` helper converts enum to display string
- Used in 6 files: event.ts, EventsManagement.tsx, Home.tsx, UserProfile.steps.ts, TicketPurchase.tsx, UserProfile.tsx
- Starting fresh - no migration of hardcoded venues to API

**API Contract:**
```typescript
// Venue Schema
{
  id: string;           // UUID
  name: string;         // "The Grand Theater"
  address: {
    street: string;     // "123 Main Street"
    city: string;       // "London"
    postCode: string;   // "SW1A 1AA"
  };
  capacity: number;     // 25
}

// Endpoints
POST   /venues        - Create venue (admin only) → 201 + Location header
GET    /venues/{id}   - Get single venue (public)
GET    /venues        - List all venues (public)
PUT    /venues/{id}   - Update venue (admin only)
```

**Event API Changes:**
- Events return `venueId: string` (just ID, not full venue object)
- Creating events sends `venueId` in payload
- Need separate fetch to get venue details for display

**Decisions to Make:**
- Where to place VenueDisplay component? (`src/components/` vs `src/views/components/`)
- How to handle venue loading states in event displays?
- Should we cache venue data to avoid repeated fetches?

**TypeScript Requirements:**
- Schema-first at API boundary (Zod schemas for API responses)
- Derive types from schemas
- No `any` types
- Strict mode compliance

**Testing Requirements:**
- TDD for all code (RED-GREEN-REFACTOR)
- Test behavior through public APIs
- Factory functions for test data
- Schema validation in tests (import schemas, don't redefine)

## Session Log

### 2026-01-05 - Session 1
**Duration**: Planning + Phase 1
**Completed**:
- Analyzed current venue enum implementation
- Identified 6 files using Venue enum/helper
- Created comprehensive WIP document with 28 steps across 6 phases
- Installed Zod for schema validation
- Created venue domain types (TDD): `src/domain/venue.ts`
  - VenueSchema, VenuePayloadSchema, AddressSchema
  - Types: Venue, VenuePayload, Address
- Created venue API client (TDD): `src/api/venues.api.ts`
  - getVenues(), getVenueById(), createVenue(), updateVenue()
  - Schema validation at API boundaries
- All 66 tests passing

**Files Created**:
- `src/domain/venue.ts` - Venue domain types with Zod schemas
- `src/domain/unit_specs/venue.spec.ts` - 5 tests
- `src/api/venues.api.ts` - Venue API client
- `src/api/unit_specs/venues.api.spec.ts` - 5 tests

**Next Session**:
- Begin Phase 2: Venues Management UI
- Create VenuesManagement component with TDD
