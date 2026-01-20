# WIP: User Notifications System

**Started**: 2026-01-20
**Status**: Implementation Phase
**Current Step**: 7 of 9 (Phase 2 complete, ready for Phase 3)

## Goal

Introduce a user notifications system to inform users about important events. Initial implementation focuses on ticket purchase notifications delivered via browser polling. The system is designed for extensibility to support additional notification types (e.g., event updates, price changes) and delivery methods (e.g., email, push notifications) in future iterations.

## Overall Plan

### Phase 1: Foundation (Steps 1-4) ✓ COMPLETE
1. ~~Create Notifications module structure~~ ✓
2. ~~Create DbUp migration for Notification schema and tables~~ ✓
3. ~~Define domain model for notifications~~ ✓ (TDD - unit tests passing)
4. ~~Create notification repository port and implementation~~ ✓

**Commits:**
- 47da880: add Notifications module structure
- f4d0c1c: add DbUp migration for Notification schema
- e2ec3da: add Notification domain model with TDD
- fe9da91: add notification repository port
- 02fd87f: add PostgreSQL notification repository with EF Core

### Phase 2: Vertical Slices - API Endpoints (Steps 5-7) ✓ COMPLETE
Each step: Write failing integration test → implement full vertical slice → green → commit

5. ~~**GET /notifications for user**~~ ✓
   - Integration test: request notifications for user, expect list ordered by CreatedAt DESC
   - Implemented: Controller → GetNotifications use case → Repository (EF Core)
   - Commits: f9ae5fc (green), a61ecab (refactor: shared ClaimsPrincipalExtensions), e53b0a5 (refactor: Routes.cs)

6. ~~**POST /notifications/{id}/read** (mark as read)~~ ✓
   - Integration test: mark notification as read, verify state change
   - Implemented: Controller → MarkNotificationAsRead use case → Repository with UnitOfWork
   - Commits: c470901 (green), 7efddf8 (refactor)

7. ~~**GET /notifications/unread-count**~~ ✓
   - Integration test: request unread count for user
   - Implemented: Controller → GetUnreadCount use case → Repository (GetUnreadCountByUserId)
   - Commits: fe0eccc (green), dc97351 (refactor: test data factory)

### Phase 3: Ticket Purchase Notification (Steps 8-9)
8. TicketPurchased event + handler creates notification
   - Integration test: purchase ticket → verify notification created
   - Implement: Domain event, publish from PurchaseTickets, consumer, create notification

9. End-to-end flow test
   - Integration test: purchase → notification appears in GET /notifications

**Note:** Repository was implemented in earlier session without TDD. It will be exercised and validated through the integration tests in Phase 2.

## Current Focus

**Status**: Phase 2 complete (7/9 steps complete). Ready for Phase 3: Ticket Purchase Notification

**Next Action**: Write failing integration test for Step 8 (TicketPurchased event → notification created)

**Architecture Decisions to Document**:
- Notifications as separate bounded context (new module)
- REST API controllers with browser polling (WebSockets/SSE considered for future)
- PostgreSQL with new "Notification" schema (DbUp migration)
- JSONB for notification payload if needed (extensible notification types)
- Domain events for cross-module communication (Tickets -> Notifications)

## Agent & Skill Checkpoints

**Agents (invoke explicitly for fresh-context critique):**
- [ ] tdd-guardian: Invoke before each commit (15 steps expected)
- [ ] refactor-scan: Invoke after GREEN phase for each step

**Skills (auto-trigger based on context):**
- [ ] adr: Will trigger for architecture decisions (module structure, polling vs push, storage strategy)
- [ ] learn: Will capture cross-module messaging patterns, notification design patterns

## Next Steps

1. Start Step 1: Create Notifications module structure following existing module conventions
2. Set up project structure (Domain, Application, Infrastructure, Controllers, Messages, Messaging, Testing)
3. Wire up module in solution and host

## Blockers

None currently

## Technical Notes

**Existing Architecture Context**:
- Modular monolith with three existing modules: Events, Tickets, Keycloak.Users
- Hexagonal architecture (ports & adapters)
- CQRS with command/query separation
- Domain events via MassTransit messaging
- Each module has: Domain, Application, Infrastructure, Controllers, Messages, Messaging, Testing layers

**Current Tickets Module Context**:
- PurchaseTickets use case exists (Application.Tickets.Ticket.PurchaseTickets)
- Ticket domain entity has Purchase(userId) method
- Currently NO domain event published on ticket purchase
- Messaging infrastructure already exists (Messages.Tickets, Messaging.Tickets)
- Example: EventSoldOut message already defined in Messages.Tickets

**Key Design Decisions**:
- Notifications module will be a new bounded context
- Cross-module communication via domain events (established pattern)
- TicketPurchased event will be new message in Messages.Tickets
- Notification handler in Notifications module will consume TicketPurchased
- REST API controllers in Notifications module for polling
- No real-time push for v1 (WebSockets/SSE considered for future iterations)

**Database Strategy**:
- New "Notification" schema in PostgreSQL (following Event/Ticket pattern)
- DbUp migration script (next: 015-CreateNotificationSchema.sql)
- EF Core with NotificationDbContext for writes (UnitOfWork pattern)
- Dapper for read queries (NotificationQuerist)
- JSONB column available for notification payload if needed (extensibility)

**Extensibility Considerations**:
- Notification type as discriminator (enables multiple notification types)
- Delivery method abstraction (enables email, SMS, push later)
- Notification status lifecycle (unread -> read -> archived)
- User preference system (which notifications to receive) - future

## Session Log

### 2026-01-20 - Session 1 (Planning + Phase 1 Complete)
**Duration**: Planning phase + Steps 1-4 implementation
**Completed**:
- Analyzed existing codebase structure
- Identified module boundaries
- Designed 9-step incremental plan
- Identified architecture decision points
- **Step 1**: Created Notifications module structure (10 projects) - commit 47da880
- **Step 2**: Created DbUp migration (015-CreateNotificationSchemaAndTable.sql) - commit f4d0c1c
- **Step 3**: Defined Notification domain model with TDD (5 tests passing) - commit e2ec3da
- **Step 4**: Created IPersistNotifications port and NotificationRepository - commits fe9da91, 02fd87f

**Phase 1 Deliverables**:
- Module structure: 10 projects (Domain, Application, Infrastructure, Controllers, Messages, Messaging, Architecture, Testing.Unit, Testing.Integration, Testing.Architecture)
- Database: Notification schema with Notifications table (Id, UserId, Type, Payload, IsRead, CreatedAt)
- Domain model: Notification entity with Create factory method and MarkAsRead behavior
- Repository: IPersistNotifications port with EF Core implementation (NotificationRepository)
- Tests: 5 unit tests passing (Notification.specs.cs)

**Learned**:
- Tickets module already has messaging infrastructure
- PurchaseTickets use case does not publish domain event yet
- Need to add TicketPurchased event to Messages.Tickets
- Notification module follows existing module conventions
- DbUp for migrations, EF Core for writes, Dapper for reads (established pattern)
- Repository implemented without TDD (will be validated via integration tests in Phase 2)

**Next Session**:
- Start Phase 2: Vertical slices with integration tests
- Step 5: Write failing integration test for GET /notifications endpoint
- Implement full vertical slice (Controller → Query → Repository)

**Agent/Skill Activity**:
- wip-guardian: Created WIP.md, updated after Phase 1 completion
- Planned to invoke tdd-guardian for Phase 2 integration tests

### 2026-01-20 - Session 2 (Step 5 Complete)
**Duration**: Step 5 implementation with TDD
**Completed**:
- **Step 5**: GET /notifications endpoint with full TDD cycle
  - RED: Wrote failing integration test (seeds notifications, calls endpoint, asserts ordering)
  - GREEN: Implemented GetNotificationsEndpoint, NotificationResponse, GetNotifications use case
  - REFACTOR: Extracted ClaimsPrincipalExtensions to CommonLibraries/Application (eliminated duplication with Tickets)
  - REFACTOR: Added Routes.cs pattern for centralized route constants
  - Commits: f9ae5fc (green), a61ecab (refactor: ClaimsPrincipalExtensions), e53b0a5 (refactor: Routes.cs)

**Files Created**:
- Controllers.Notifications/GetNotificationsEndpoint.cs
- Controllers.Notifications/NotificationResponse.cs
- Controllers.Notifications/Routes.cs
- Application.Notifications/GetNotifications.cs
- Testing.Integration.Notifications/Setup.cs
- Testing.Integration.Notifications/NotificationController.specs.cs
- Testing.Integration.Notifications/NotificationController.steps.cs
- CommonLibraries/Application/Authentication/ClaimsPrincipalExtensions.cs (shared)

**Agent/Skill Activity**:
- tdd-guardian: Verified RED-GREEN-REFACTOR cycle, identified DI configuration issue
- refactor-scan: Identified ClaimsPrincipalExtensions duplication (critical) and Routes.cs pattern (high value)
- wip-guardian: Updated WIP.md with Step 5 completion

**Next Session**:
- Step 6: POST /notifications/{id}/read (mark as read)

### 2026-01-20 - Session 2 continued (Step 6 Complete)
**Duration**: Step 6 implementation with TDD
**Completed**:
- **Step 6**: POST /notifications/{id}/read endpoint with full TDD cycle
  - RED: Wrote failing integration test (creates notification, marks as read, verifies IsRead=true)
  - GREEN: Implemented MarkNotificationAsReadEndpoint, MarkNotificationAsRead use case
  - Created UnitOfWork pattern (INotificationsUnitOfWork + UnitOfWork impl) following Tickets pattern
  - REFACTOR: Removed redundant ordering in GetNotifications, extracted PersistNotification test helper
  - Commits: c470901 (green), 7efddf8 (refactor)

**Files Created**:
- Domain.Notifications/INotificationsUnitOfWork.cs
- Infrastructure.Notifications/Core/UnitOfWork.cs
- Application.Notifications/MarkNotificationAsRead.cs
- Controllers.Notifications/MarkNotificationAsReadEndpoint.cs

**Agent/Skill Activity**:
- tdd-guardian: Verified RED-GREEN-REFACTOR cycle
- refactor-scan: Identified ordering duplication (critical) and test helper extraction (high value)
- wip-guardian: Updated WIP.md with Step 6 completion

**Next Session**:
- Step 7: GET /notifications/unread-count

### 2026-01-20 - Session 3 (Step 7 Complete - Phase 2 Done)
**Duration**: Step 7 implementation with TDD
**Completed**:
- **Step 7**: GET /notifications/unread-count endpoint with full TDD cycle
  - RED: Wrote failing integration test (creates unread/read notifications, calls endpoint, asserts count=2)
  - GREEN: Implemented GetUnreadCountEndpoint, GetUnreadCount use case, added GetUnreadCountByUserId to repository
  - REFACTOR: Extracted CreateNotification test data factory
  - Commits: fe0eccc (green), dc97351 (refactor: test data factory)

**Files Created/Modified**:
- Domain.Notifications/IPersistNotifications.cs (added GetUnreadCountByUserId)
- Infrastructure.Notifications/Notification/NotificationRepository.cs (implemented GetUnreadCountByUserId)
- Application.Notifications/GetUnreadCount.cs (new)
- Controllers.Notifications/GetUnreadCountEndpoint.cs (new)
- Controllers.Notifications/Routes.cs (added UnreadCount)

**Phase 2 Summary**:
All 3 API endpoints complete with integration tests:
- GET /notifications - list notifications for user (ordered by CreatedAt DESC)
- POST /notifications/{id}/read - mark notification as read
- GET /notifications/unread-count - count of unread notifications for user

**Agent/Skill Activity**:
- tdd-guardian: Verified RED-GREEN-REFACTOR cycle
- refactor-scan: Identified test data factory opportunity (high value)
- wip-guardian: Updated WIP.md with Step 7 and Phase 2 completion

**Next Session**:
- Phase 3: Step 8 (TicketPurchased event + handler creates notification)
