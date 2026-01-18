# ADR Template

Complete template for Architecture Decision Records.

```markdown
# ADR-NNN: [Short Title]

**Status**: Accepted | Proposed | Deprecated | Superseded by ADR-XXX

**Date**: YYYY-MM-DD

**Decision Makers**: [Who was involved]

**Tags**: [relevant, tags, for, searching]

## Context

[What is the issue we're addressing? What factors are influencing this decision?]

- Current situation
- Problem to solve
- Constraints
- Requirements

## Decision

[What did we decide? State it clearly and concisely.]

We will [decision statement].

## Alternatives Considered

### Alternative 1: [Name]

**Pros:**
- Advantage 1
- Advantage 2

**Cons:**
- Disadvantage 1
- Disadvantage 2

**Why Rejected**: [Specific reason]

### Alternative 2: [Name]

**Pros:**
- Advantage 1

**Cons:**
- Disadvantage 1

**Why Rejected**: [Specific reason]

## Consequences

### Positive

- [Good consequence 1]
- [Good consequence 2]

### Negative

- [Trade-off 1]
- [Trade-off 2]

### Neutral

- [Other impact 1]

## Implementation Notes

- [How will this be implemented?]
- [What needs to change?]
- [Timeline considerations]

## Related Decisions

- [ADR-XXX] - Related decision
- [ADR-YYY] - Another related decision

## References

- [Relevant documentation]
- [Articles or research that informed this decision]
```

## ADR Index Template

Keep `docs/adr/README.md` updated:

```markdown
# Architecture Decision Records

This directory contains Architecture Decision Records (ADRs) documenting significant architectural choices.

## Active ADRs

- [ADR-001: Email Provider Selection](001-email-provider-selection.md) - 2025-11-01
- [ADR-002: Job Queue Infrastructure](002-job-queue-infrastructure.md) - 2025-11-02

## Superseded ADRs

- [ADR-000: Initial Architecture](000-initial-architecture.md) - Superseded by ADR-001

## Tags

- **infrastructure**: ADR-001, ADR-002
- **frontend**: ADR-003
- **backend**: ADR-001, ADR-002
```

## Good ADR Characteristics

- ✅ Clear problem statement
- ✅ Specific alternatives with trade-offs
- ✅ Honest about negative consequences
- ✅ Explains the "why" behind the decision
- ✅ Actionable implementation notes

## Poor ADR Characteristics

- ❌ Vague problem statement
- ❌ Only one option considered
- ❌ Doesn't explain rationale
- ❌ Ignores negative consequences
- ❌ No implementation guidance