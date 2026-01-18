---
name: adr
description: >
  Creates Architecture Decision Records for significant choices. Auto-triggers when: evaluating
  technology options (database, framework, library), making foundational decisions (architecture
  patterns, infrastructure), discussing trade-offs between alternatives, or someone asks
  "why did we choose X?" Stores ADRs in docs/adr/.
allowed-tools:
  - Read
  - Write
  - Edit
  - Grep
  - Glob
  - Bash
---

# Architecture Decision Records

Creates ADRs for significant architectural choices, capturing context, alternatives, and consequences.

**Core Philosophy:**
- Permanent documentation in the repository
- Capture WHY, not just WHAT
- Document alternatives and trade-offs
- Use judiciously - only for significant decisions

## Quick Decision Framework

**Create an ADR if 3+ are YES:**

1. Is this a one-way door? (Hard/expensive to reverse)
2. Did I evaluate alternatives? (Considered trade-offs)
3. Will this affect future architectural decisions?
4. Will future developers wonder "why did they do it this way?"
5. Is this NOT covered by existing guidelines/ADRs?

## DO Create ADRs For

- System architecture patterns (microservices, event-driven)
- Data storage decisions (SQL vs NoSQL, specific database)
- Technology/library selections with long-term impact
- Pattern decisions affecting multiple modules
- Performance vs maintainability trade-offs
- Security architecture decisions

## DON'T Create ADRs For

- Trivial implementation choices (naming, parameter order)
- Temporary workarounds or spikes
- Standard patterns already in CLAUDE.md
- Decisions with no alternatives considered
- Decisions that will change frequently

## ADR Format (Quick Reference)

```markdown
# ADR-NNN: [Short Title]

**Status**: Accepted | Proposed | Deprecated | Superseded by ADR-XXX
**Date**: YYYY-MM-DD
**Tags**: [relevant, tags]

## Context
[Problem, constraints, requirements]

## Decision
We will [decision statement].

## Alternatives Considered
### Alternative 1: [Name]
**Pros:** ...
**Cons:** ...
**Why Rejected**: [Specific reason]

## Consequences
### Positive
### Negative
### Neutral

## Implementation Notes
## Related Decisions
## References
```

See @adr-template.md for the complete template.

## Process

1. **Determine next ADR number:**
   ```bash
   ls docs/adr/ | grep -E '^[0-9]+' | sort -n | tail -1
   ```

2. **Gather context:**
   - What needs to be decided?
   - What options were considered?
   - What trade-offs exist?
   - Why was this chosen?

3. **Create ADR file:** `docs/adr/NNN-descriptive-title.md`

4. **Update index:** `docs/adr/README.md`

## Reference Files

- @adr-template.md - Complete ADR format with all sections
- @decision-framework.md - Detailed criteria for when to create ADRs
- @examples.md - Example ADRs and anti-patterns to avoid

## Integration

**With learn agent:** ADRs document WHY (architecture), CLAUDE.md documents HOW (patterns/gotchas)

**Retroactive ADRs:** When someone asks "why did we...?" and no ADR exists, create one to capture the rationale.

## Your Mandate

Create ADRs that answer the question future developers will ask: "Why did they do it this way?"

Be selective - not every decision needs an ADR. But when one is needed, be thorough about context, alternatives, and consequences.