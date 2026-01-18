# ADR Examples

## Good Example: Technology Selection

```markdown
# ADR-001: Email Provider Selection

**Status**: Accepted
**Date**: 2025-11-01
**Tags**: infrastructure, email, third-party-service

## Context

We need to send transactional emails (password resets, notifications) and marketing emails. Requirements:

- Reliable delivery (>99% delivery rate)
- Template support
- Analytics/tracking
- Reasonable pricing (<$100/month for current volume)
- Good developer experience

## Decision

We will use **SendGrid** for all email sending.

## Alternatives Considered

### Alternative 1: AWS SES

**Pros:**
- Very cheap ($0.10 per 1000 emails)
- Already using AWS for other services

**Cons:**
- More complex setup
- Less developer-friendly API
- No built-in template management

**Why Rejected**: Higher implementation cost due to complexity.

### Alternative 2: Self-hosted (Postfix)

**Pros:**
- No per-email costs
- Complete control

**Cons:**
- Significant maintenance burden
- Deliverability challenges

**Why Rejected**: Maintenance burden too high for team size.

## Consequences

### Positive
- Leverages existing team experience
- Good developer experience
- Built-in analytics

### Negative
- Higher cost than AWS SES
- Vendor lock-in (SendGrid-specific templates)

## Implementation Notes
- Install SendGrid package
- Configure API key in environment variables
- Set up SPF/DKIM records
```

## Good Example: Retroactive ADR

When someone asks "why did we choose X?" and no ADR exists:

```markdown
# ADR-003: Database Selection - PostgreSQL

**Status**: Accepted
**Date**: 2025-11-05 (Decision made: 2025-10-15)
**Tags**: database, infrastructure, retroactive

**Note**: This ADR created retroactively to document initial decision.

## Context

(When the decision was made)

Needed primary database with requirements:
- ACID transactions for payment processing
- Complex queries with joins
- Strong consistency guarantees

## Decision

We chose **PostgreSQL** as the primary database.

## Alternatives Considered

### Alternative 1: MongoDB

**Pros:**
- Flexible schema
- Horizontal scaling

**Cons:**
- Weaker consistency guarantees
- Team less experienced with NoSQL

**Why Rejected**: Data is highly relational. ACID critical for payments.

## Consequences

### Positive (as experienced)
- Strong consistency for payments ✅
- Excellent TypeScript integration via Prisma ✅

### Negative
- Horizontal scaling more complex than NoSQL
```

## Anti-Patterns to Avoid

### ❌ ADR for Trivial Choice

```markdown
# Bad: ADR-042: Use var instead of string

## Decision
We will use `var` instead of `string` for variable declarations.
```

**Why bad**: Code style guideline, not architecture. Belongs in CLAUDE.md.

### ❌ ADR Without Alternatives

```markdown
# Bad: ADR-015: Use React

## Decision
We will use React.

## Alternatives Considered
None. React is the obvious choice.
```

**Why bad**: If no alternatives, it's not a decision. Explain why React vs Vue/Svelte.

### ❌ ADR Without Rationale

```markdown
# Bad: ADR-023: Use BullMQ

## Decision
We will use BullMQ for job queuing.

## Consequences
We'll have a job queue.
```

**Why bad**: Doesn't explain why BullMQ, what problems it solves, or trade-offs.

### ❌ ADR for Existing Guidelines

```markdown
# Bad: ADR-031: Follow TDD

## Decision
We will follow Test-Driven Development.
```

**Why bad**: TDD is already non-negotiable in CLAUDE.md. Doesn't need ADR.

## Rejecting an ADR Request

When a decision doesn't warrant an ADR:

```
User: "I decided to use camelCase for function names"

Response: "That's a code style convention, not an architectural decision.
This should be documented in CLAUDE.md or a style guide, but doesn't
need an ADR. ADRs are for significant architectural choices with
trade-offs and long-term consequences."
```