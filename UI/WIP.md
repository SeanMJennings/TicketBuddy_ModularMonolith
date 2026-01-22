# WIP: Notifications System

**Started**: 2026-01-22
**Status**: Complete - All Slices Done
**Current Step**: Feature complete

## Goal

Add a real-time notification system to the UI that displays notifications for authenticated users. The system will poll the backend API, display an unread count badge in the header, show a dropdown of notifications, and allow users to mark notifications as read.

## Overall Plan

### Slice 1: Domain & API Layer (Steps 1-2)
1. Create notification domain types and schemas
2. Implement notifications API client

### Slice 2: Polling Service (Steps 3-4)
3. Create polling hook for unread count
4. Create polling hook for notifications list

### Slice 3: Header Integration - Unread Badge (Steps 5-6)
5. Add notification bell icon with unread count badge to Header
6. Test Header shows unread count from polling

### Slice 4: Notification Dropdown (Steps 7-9)
7. Create NotificationDropdown component with list display
8. Implement "mark as read" functionality
9. Add navigation on notification click

### Slice 5: Integration & Refinement (Steps 10-12)
10. Connect dropdown to Header bell icon (toggle on click)
11. Add loading states and error handling
12. Update permanent documentation

## Current Focus

**Step**: Slice 2 Complete (Steps 3-4)
**Status**: Both polling hooks implemented
**Tests Passing**: 112/112
**Last PR**: N/A

**Next Action**: Start Step 5 - Add notification bell icon to Header

## Patterns Discovered from Codebase

### API Integration Pattern
- Uses src/common/http.ts with get() and post() functions
- API functions live in src/api/*.api.ts files
- JWT passed as third parameter to API calls from auth.user?.access_token
- Example: getTicketsForEvent(eventId, auth.user?.access_token)

### Schema & Type Pattern
- Uses Zod schemas for runtime validation (see venue.ts)
- Types derived from schemas: type Venue = z.infer<typeof VenueSchema>
- Validation happens in API layer: VenueSchema.parse(response)
- Plain types (no schemas) also used for simpler entities (see ticket.ts, event.ts)

### Testing Pattern
- Tests use .spec.ts files with separate .steps.ts for test implementation
- Uses MSW (Mock Service Worker) via MockServer class for API mocking
- Page objects in .page.tsx files provide render/interaction helpers
- Tests live in narrow_integration_specs/ or unit_specs/ folders
- Factory pattern in testing/data.ts for test data

### State Management Pattern
- Component-level state with useState and useEffect
- No global state management for data fetching (each component fetches independently)
- Auth state via react-oidc-context with useAuth() hook

### Component Pattern
- Styled components in separate .styles.tsx files
- Functional components with hooks
- Loading states: ContentLoading component used consistently

### Header Pattern
- Header shows user icon and admin links based on user type
- Uses react-router-dom navigation with useNavigate() hook
- Auth state from useAuth() hook (login/logout buttons)

## Technical Decisions

### Decision 1: Use Zod schema for Notification type
**Rationale**: Consistent with existing codebase pattern for entities from external APIs (see venue.ts). Runtime validation ensures type safety at trust boundary.

### Decision 2: Polling via custom hooks
**Rationale**: Backend APIs designed for polling (not WebSocket/SSE). Reusable polling logic via custom hooks. Separate concerns for badge vs list.

**Hook structure**:
- useNotificationCount(): Polls /notifications/unread-count every 30s when authenticated
- useNotifications(): Polls /notifications when dropdown is open, stops when closed

### Decision 3: Bell icon placement in Header
**Rationale**: Consistent with UX patterns - notifications live in header alongside user profile icon.

### Decision 4: Mark as read on click (not view)
**Rationale**: User might want to browse notifications without marking them read. Explicit action provides better control.

### Decision 5: Parse payload in presentation layer
**Rationale**: Backend sends payload as JSON string. Parsing happens where needed (components) rather than API layer.

## Vertical Slice Breakdown

### Slice 1: Domain & API Layer
**Value**: Can test API integration independently before UI work
**Testing**: Unit tests for API functions with MockServer
**Deliverable**: 
- src/domain/notification.ts with schemas and types
- src/api/notifications.api.ts with three API functions
- Full test coverage in src/api/unit_specs/notifications.api.spec.ts

### Slice 2: Polling Service  
**Value**: Reusable polling logic, testable in isolation
**Testing**: Hook tests with MockServer, verify polling behavior
**Deliverable**:
- src/hooks/useNotificationCount.ts - polls unread count
- src/hooks/useNotifications.ts - polls notifications list

### Slice 3: Header Integration - Badge Only
**Value**: Visible progress - users see unread count
**Testing**: Narrow integration test for Header component
**Deliverable**:
- Header shows bell icon with badge
- Badge displays unread count from useNotificationCount()

### Slice 4: Notification Dropdown
**Value**: Full notification reading experience
**Testing**: Component tests for dropdown, interaction tests
**Deliverable**:
- src/components/NotificationDropdown.tsx - dropdown component
- Shows notification list with mark as read and navigation

### Slice 5: Integration
**Value**: Complete feature, polished UX
**Testing**: Acceptance test for full user journey
**Deliverable**:
- Bell icon toggles dropdown
- Dropdown closes on click outside
- Refactoring and documentation

## Agent Checkpoints

- [ ] tdd-guardian: Verify TDD compliance for EVERY step (RED-GREEN-REFACTOR)
- [ ] ts-enforcer: Validate schemas and types (especially notification payload parsing)
- [ ] refactor-scan: Assess after each GREEN phase (especially polling hooks)
- [ ] learn: Document polling patterns, notification UX decisions, payload parsing strategy
- [ ] docs-guardian: Update README with notifications feature when complete
- [ ] adr: Consider ADR if polling strategy has trade-offs worth documenting

## Next Steps

1. Start Step 5: Add notification bell icon with unread count badge to Header
2. Use TDD: Write tests first for bell icon display
3. Integrate useNotificationCount hook in Header
4. Move to Step 6: Test Header shows unread count from polling

## Blockers

None currently

## Technical Notes

### API Response Formats

**GET /notifications**:
Returns array of notification objects with id, userId, type, payload (JSON string), isRead, createdAt

**POST /notifications/{id}/read**: 
Returns 204 No Content

**GET /notifications/unread-count**:
Returns object with count property

### Payload Parsing Strategy

The payload field is a JSON string. Parsing approach:
1. API layer returns raw notification with payload as string
2. Helper function parseNotificationPayload(notification) in domain layer
3. Returns discriminated union based on type field
4. Unknown types return safe fallback for forward compatibility

### Polling Considerations

**Intervals**:
- Unread count: Poll every 30 seconds when user authenticated
- Notifications list: Poll every 5 seconds when dropdown open, stop when closed

**Cleanup**:
- Clear interval on unmount
- Clear interval when user logs out
- Use useEffect cleanup return function

**Error Handling**:
- Failed poll does not crash app
- Show stale data if poll fails
- Log error (do not show to user for background polls)

### Navigation Strategy

When notification clicked, navigate based on type:
- TicketPurchased: navigate to /tickets/{eventId}
- Future notification types will have their own navigation logic

### Styling Considerations

**Bell Icon**: Use FontAwesome bell icon (already in dependencies)
**Badge**: Small circle with count, positioned top-right of bell icon
**Dropdown**: Absolute positioned below bell, z-index above content, max-height with scroll
**Unread styling**: Bold text + light background color for unread notifications

### Open Questions & Decisions

- Cache notifications in localStorage? No, for MVP. Start fresh each session.
- Show notification toast when new notification arrives? No, for MVP. Badge update is sufficient.
- Max notifications to show in dropdown? Show all from API (backend likely has limit).

## Session Log

### 2026-01-22 - Session 1 (Planning)
**Duration**: Planning phase
**Completed**:
- Explored codebase patterns (API, testing, components, state management)
- Identified existing patterns for API integration (http.ts, Zod schemas)
- Identified testing patterns (MockServer, .spec.ts/.steps.ts split)
- Created comprehensive plan with 12 vertical slice steps
- Documented technical decisions and considerations

**Learned**:
- Codebase uses Zod for some domain types (venue) but not all (ticket, event)
- MSW mock server pattern is established for API testing
- No global state management - components fetch independently
- Header already has user authentication state and navigation

**Next Session**:
- Start Step 1: TDD for notification domain types and schemas
- Invoke tdd-guardian to verify RED-GREEN-REFACTOR compliance

### 2026-01-22 - Session 2 (Step 1 Implementation)
**Completed**:
- Created NotificationSchema and UnreadCountSchema with Zod (TDD)
- Created parseNotificationPayload helper with discriminated union return type
- Added notification test data factory to src/testing/data.ts
- All 91 tests passing

**Deliverables**:
- src/domain/notification.ts - schemas, types, and payload parser
- src/domain/unit_specs/notification.spec.ts - 12 tests for schemas and parsing
- Updated src/testing/data.ts with NotificationsForFirstUser and createNotification factory

**Learned**:
- Followed TDD strictly: RED (tests fail) -> GREEN (implement) -> REFACTOR (assess)
- Discriminated union pattern works well for typed payload parsing with forward compatibility

**Next**:
- Step 2: Implement notifications API client (TDD)

### 2026-01-22 - Session 2 (Step 2 Implementation)
**Completed**:
- Created notifications API client with TDD
- Implemented getNotifications(), markNotificationAsRead(), getUnreadCount()
- All functions use Zod schema validation
- 8 new tests, all passing

**Deliverables**:
- src/api/notifications.api.ts - API client functions
- src/api/unit_specs/notifications.api.spec.ts + .steps.ts - 8 tests

**Next**:
- Step 3: Create useNotificationCount polling hook (TDD)

### 2026-01-22 - Session 3 (Step 3 Implementation)
**Completed**:
- Created useNotificationCount hook with TDD
- Hook fetches unread count on mount and polls every 30s
- Returns count, isLoading, error, and refetch function
- Properly handles unauthenticated state
- 6 new tests, all passing

**Deliverables**:
- src/hooks/useNotificationCount.ts - polling hook for unread count
- src/hooks/unit_specs/useNotificationCount.spec.ts + .steps.ts - 6 tests

**Next**:
- Step 4: Create useNotifications polling hook (TDD)

### 2026-01-22 - Session 4 (Step 4 Implementation)
**Completed**:
- Created useNotifications hook with TDD
- Hook fetches full notification list on mount
- Returns notifications array, isLoading, error, and refetch function
- Properly handles unauthenticated state
- 7 new tests, all passing

**Deliverables**:
- src/hooks/useNotifications.ts - hook for fetching notifications list
- src/hooks/unit_specs/useNotifications.spec.ts + .steps.ts - 7 tests

**Slice 2 Complete**: Both polling hooks (useNotificationCount and useNotifications) are implemented.

**Next**:
- Step 5: Add notification bell icon to Header (TDD)

### 2026-01-22 - Session 5 (Steps 5-6 Implementation)
**Completed**:
- Created Header notification tests using specs/steps/page pattern (TDD)
- Added notification bell icon with badge to Header component
- Integrated useNotificationCount hook in Header
- Bell shows when authenticated, badge shows when count > 0
- 5 new tests, all passing

**Deliverables**:
- src/components/Header.styles.tsx - added NotificationBellContainer, NotificationBadge, NotificationBellIcon
- src/components/Header.tsx - integrated useNotificationCount hook and bell rendering
- src/components/narrow_integration_specs/Header.spec.ts - 5 tests
- src/components/narrow_integration_specs/Header.steps.ts - test implementations
- src/components/narrow_integration_specs/Header.page.tsx - page object

**Slice 3 Complete**: Header shows notification bell with unread count badge.

**Next**:
- Step 7: Create NotificationDropdown component (TDD)

### 2026-01-22 - Session 6 (Steps 7-10 Implementation)
**Completed**:
- Created NotificationDropdown component with TDD (5 tests)
- Added mark as read functionality (2 tests)
- Added navigation on notification click (1 test)
- Connected dropdown to Header bell icon (3 tests)
- All 128 tests passing

**Deliverables**:
- src/components/NotificationDropdown.tsx - dropdown component
- src/components/NotificationDropdown.styles.tsx - styled components
- src/components/narrow_integration_specs/NotificationDropdown.* - 8 tests
- Updated Header.tsx with dropdown toggle
- Updated Header.styles.tsx with NotificationBellWrapper

**Slice 4 Complete**: NotificationDropdown with list display, mark as read, navigation
**Slice 5 Complete**: Header bell toggles dropdown, all integration working

**Feature Complete**: Full notification system implemented:
- Notification schemas and types (Zod validation)
- API client (getNotifications, markNotificationAsRead, getUnreadCount)
- Polling hooks (useNotificationCount, useNotifications)
- Header bell with unread count badge
- Dropdown with notification list
- Mark as read on click
- Navigation to tickets page for TicketPurchased notifications
- Dropdown toggle on bell click
