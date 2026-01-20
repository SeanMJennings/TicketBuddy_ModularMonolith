# WIP: User Notifications System

**Started**: 2026-01-20
**Status**: Planning Phase
**Current Step**: 4 of 9 (Phase 1 complete, ready for Phase 2 vertical slices)

## Goal

Introduce a user notifications system to inform users about important events. Initial implementation focuses on ticket purchase notifications delivered via browser polling. The system is designed for extensibility to support additional notification types (e.g., event updates, price changes) and delivery methods (e.g., email, push notifications) in future iterations.

## Overall Plan

### Phase 1: Foundation (Steps 1-4) ✓
1. ~~Create Notifications module structure~~ ✓
2. ~~Create DbUp migration for Notification schema and tables~~ ✓
3. ~~Define domain model for notifications~~ ✓ (TDD - unit tests)
4. ~~Create notification repository port (interface)~~ ✓

### Phase 2: Vertical Slices - API Endpoints (Steps 5-7)
Each step: Write failing integration test → implement full vertical slice → green → commit

5. GET /notifications for user
   - Integration test: request notifications for user, expect list
   - Implement: Controller → Query service → Repository (EF/Dapper)

6. POST /notifications/{id}/read (mark as read)
   - Integration test: mark notification as read, verify state change
   - Implement: Controller → Use case → Repository

7. GET /notifications/unread-count
   - Integration test: request unread count for user
   - Implement: Controller → Query service

### Phase 3: Ticket Purchase Notification (Steps 8-9)
8. TicketPurchased event + handler creates notification
   - Integration test: purchase ticket → verify notification created
   - Implement: Domain event, publish from PurchaseTickets, consumer, create notification

9. End-to-end flow test
   - Integration test: purchase → notification appears in GET /notifications

**Note:** Repository was implemented in earlier session without TDD. It will be exercised and validated through the integration tests in Phase 2.

## Current Focus

**Status**: Phase 1 complete, ready for Step 5 (GET /notifications vertical slice with integration test)

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

### 2026-01-20 - Session 1 (Planning + Step 1)
**Duration**: Planning phase + Step 1 implementation
**Completed**:
- Analyzed existing codebase structure
- Identified module boundaries
- Designed 16-step incremental plan
- Identified architecture decision points
- **Step 1**: Created Notifications module structure (10 projects)

**Step 1 Details**:
- Created 10 projects: Domain, Application, Infrastructure, Controllers, Messages, Messaging, Architecture, Testing.Unit, Testing.Integration, Testing.Architecture
- Added all projects to solution under "Notifications" folder
- Wired up module in Host (Host.csproj, Services.cs, Messaging.cs)
- Build succeeded with 0 errors

**Learned**:
- Tickets module already has messaging infrastructure
- PurchaseTickets use case does not publish domain event yet
- Need to add TicketPurchased event to Messages.Tickets
- Notification module will follow existing module conventions
- DbUp for migrations, EF Core for writes, Dapper for reads

**Next Session**:
- Step 2: Create DbUp migration for Notification schema and tables
- Follow TDD for domain model (Step 3+)

**Agent/Skill Activity**:
- wip-guardian: Created WIP.md
- Explore agent: Analyzed module structure and database patterns
