# ADR-001: Notifications as Separate Bounded Context

**Status**: Accepted
**Date**: 2026-01-20
**Tags**: architecture, notifications, bounded-context, domain-events

## Context

The TicketBuddy application needed a user notifications system to inform users about important events, starting with ticket purchase confirmations. Key requirements:

- Users need to see notifications about their ticket purchases
- Notifications must be accessible via API for browser polling
- The system should be extensible to support additional notification types (event updates, price changes) and delivery methods (email, push) in future
- Cross-module communication must respect bounded context boundaries

The existing architecture is a modular monolith with three modules (Events, Tickets, Keycloak.Users), each following hexagonal architecture with ports and adapters, communicating via MassTransit domain events over RabbitMQ.

## Decision

We will implement Notifications as a **separate bounded context** (new module) that:

1. **Owns its own data** - Separate PostgreSQL schema ("Notification") with its own DbContext
2. **Communicates via domain events** - Consumes `TicketPurchased` events from Tickets module via MassTransit
3. **Exposes REST API with browser polling** - Three endpoints for listing, marking as read, and counting unread notifications
4. **Follows established module conventions** - Same project structure as existing modules (Domain, Application, Infrastructure, Controllers, Messages, Messaging)

### Architecture Overview

```
Tickets Module                          Notifications Module
┌─────────────────┐                    ┌─────────────────┐
│ PurchaseTickets │                    │ TicketPurchased │
│    Use Case     │                    │    Consumer     │
└────────┬────────┘                    └────────┬────────┘
         │                                      │
         │ raises domain event                  │ creates
         ▼                                      ▼
┌─────────────────┐    publishes      ┌─────────────────┐
│ TicketWas       │───────────────────▶│ Notification    │
│ PurchasedHandler│  TicketPurchased  │ (Domain Entity) │
└─────────────────┘   (integration)   └─────────────────┘
```

### API Design

- `GET /notifications` - List notifications for authenticated user (ordered by CreatedAt DESC)
- `POST /notifications/{id}/read` - Mark notification as read
- `GET /notifications/unread-count` - Get count of unread notifications

All endpoints require `[Authorize(Roles = Roles.Customer)]`.

## Alternatives Considered

### Alternative 1: Notifications as Part of Tickets Module

**Pros:**
- Simpler initial implementation
- Direct access to ticket/event data
- No cross-module messaging needed

**Cons:**
- Violates single responsibility - Tickets module would handle both ticketing AND notifications
- Harder to extend to other notification sources (Events module changes, user registration, etc.)
- Tighter coupling between notification logic and ticket logic

**Why Rejected**: Notifications are a cross-cutting concern that will eventually consume events from multiple modules. Embedding in Tickets would create a "god module" and make future extensions awkward.

### Alternative 2: Real-time Push (WebSockets/SSE)

**Pros:**
- Better user experience with instant updates
- Lower server load than polling
- More modern approach

**Cons:**
- Significantly more infrastructure complexity
- Connection management challenges
- Not required for MVP - users can poll

**Why Rejected**: Polling is sufficient for v1. The architecture supports adding WebSocket/SSE delivery later by adding a new delivery adapter without changing the core notification domain.

### Alternative 3: Shared Database Schema

**Pros:**
- Simpler queries joining notification and ticket/event data
- No data duplication

**Cons:**
- Tight coupling between modules at the database level
- Schema changes in one module affect others
- Violates bounded context principle

**Why Rejected**: Each module should own its data. The notification stores denormalized event name in the payload, accepting eventual consistency for true bounded context isolation.

## Consequences

### Positive

- **Clear separation of concerns** - Notifications module can evolve independently
- **Extensible** - Easy to add new notification types by creating new consumers for different events
- **Testable** - Consumer logic can be tested in isolation from the full messaging infrastructure
- **Consistent architecture** - Follows the same patterns as existing modules

### Negative

- **Data denormalization** - Event name stored in notification payload (could become stale if event is renamed)
- **Additional infrastructure** - New database schema, new message queue bindings
- **Eventual consistency** - Notification creation is asynchronous; brief delay after purchase

### Neutral

- **Browser polling** - Acceptable for v1, can be enhanced later
- **Module count** - Now 4 modules instead of 3, but each has clear responsibility

## Implementation Notes

- Domain event `TicketWasPurchased` raised from `Ticket.Purchase()` method
- Handler `TicketWasPurchasedHandler` looks up event name and publishes `TicketPurchased` integration event
- Consumer `TicketPurchasedConsumer` creates notification via `CreateTicketPurchaseNotification` use case
- Notification payload is JSON: `{"ticketId":"...", "eventId":"...", "eventName":"..."}`

## Related Decisions

- Future ADR needed if adding WebSocket/SSE delivery
- Future ADR needed if adding email notification channel

## References

- Commits: 47da880 through cd4f20c (notifications feature implementation)
- Module structure follows existing Events/Tickets module conventions
- MassTransit documentation for consumer patterns
