# Architecture Decision Records (ADRs)

This directory contains Architecture Decision Records documenting significant architectural choices for the TicketBuddy modular monolith.

## What is an ADR?

An Architecture Decision Record captures the context, decision, alternatives, and consequences of an important architectural choice. ADRs provide future developers with the "why" behind architectural decisions.

## Active ADRs

### Core Architecture

- **[ADR-001: Initial Architecture](001-initial-architecture.md)** - 2025-12-31
  - Modular monolith pattern with hexagonal architecture
  - Module structure and boundaries
  - Technology stack decisions
  - Tags: `modular-monolith`, `hexagonal-architecture`, `technology-stack`

### Code Organization

- **[ADR-003: Vertical Slicing Within Horizontal Layers](003-vertical-slicing-within-horizontal-layers.md)** - 2025-12-31
  - Hybrid approach: horizontal layers for architecture, vertical slices for behavior
  - One file per behavior pattern
  - Organizing by entity/aggregate
  - Tags: `architecture`, `organization`, `vertical-slices`, `code-organization`

### Testing

- **[ADR-002: Initial Testing](002-initial-testing.md)** - 2025-12-31
  - BDD testing pattern with specs and steps
  - Test pyramid: Unit, Integration, Architecture, Component, Acceptance
  - Testcontainers for real infrastructure
  - Tags: `testing`, `bdd`, `testcontainers`

### Integration Patterns

- **[ADR-004: Cross-Module Data Minimization](004-cross-module-data-minimization.md)** - 2026-01-04
  - Minimal data in integration messages
  - Event-carried state transfer pattern
  - Module autonomy and loose coupling
  - Tags: `modular-monolith`, `integration`, `messaging`, `cross-module`, `coupling`

## Superseded ADRs

None currently.

## Tags Index

### Architecture & Organization
- **modular-monolith**: ADR-001, ADR-004
- **hexagonal-architecture**: ADR-001
- **vertical-slices**: ADR-003
- **code-organization**: ADR-003

### Integration & Communication
- **integration**: ADR-004
- **messaging**: ADR-004
- **cross-module**: ADR-004
- **coupling**: ADR-004

### Testing
- **testing**: ADR-002
- **bdd**: ADR-002
- **testcontainers**: ADR-002

### Technology
- **technology-stack**: ADR-001

## How to Create an ADR

See `.claude/agents/adr.md` for guidance on when and how to create ADRs.

**Quick decision framework:**

Create an ADR if:
- ✅ Significant architectural choice (not trivial implementation)
- ✅ Evaluated alternatives with trade-offs
- ✅ Will affect future architectural decisions
- ✅ Future developers will wonder "why did they do it this way?"
- ✅ Not already covered by existing ADRs/guidelines

Do NOT create an ADR for:
- ❌ Trivial implementation choices
- ❌ Temporary workarounds
- ❌ Standard patterns from CLAUDE.md
- ❌ Business rules (belong in domain model)
- ❌ Decisions that will change frequently

## ADR Template

Use this structure for new ADRs:

```markdown
# ADR-NNN: [Short Title]

**Date**: YYYY-MM-DD
**Tags**: [relevant, tags]

## Context
[What is the issue? What factors are influencing this decision?]

## Decision
[What did we decide? State it clearly.]

## Alternatives Considered

### Alternative 1: [Name]
**Pros:** ...
**Cons:** ...
**Why Rejected:** ...

## Consequences

### Positive
- [Good consequence 1]

### Negative
- [Trade-off 1]

### Neutral
- [Other impact 1]

## Implementation Notes
[How will this be implemented?]

## Related Decisions
- [ADR-XXX] - Related decision

## References
- [Relevant documentation]
```

## Related Documentation

- **Architecture Guide**: `Documents/ADRs/001-initial-architecture.md`
- **Testing Guide**: `.claude/docs/testing.md`
- **Development Workflow**: `.claude/docs/workflow.md`
- **Code Style**: `.claude/docs/code-style.md`
- **DDD Patterns**: `.claude/docs/ddd-summary.md`
