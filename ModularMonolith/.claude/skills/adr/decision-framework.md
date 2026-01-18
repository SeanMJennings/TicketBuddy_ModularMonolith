# Decision Framework

Detailed criteria for when to create an ADR.

## The Five Questions

Ask these questions. If 3+ are YES → Create ADR.

| Question | YES = Consider ADR | NO = Probably Skip |
|----------|-------------------|-------------------|
| Is this a one-way door? | Hard/expensive to reverse | Easy to change later |
| Did I evaluate alternatives? | Considered trade-offs | Only one obvious way |
| Will this affect future decisions? | Foundational choice | Isolated decision |
| Will devs wonder "why this way?" | Non-obvious rationale | Self-explanatory |
| Is this NOT in existing guidelines? | New territory | Already documented |

## DO Create ADRs For

### Significant Architectural Choices
- System architecture patterns (microservices, monolith, event-driven)
- Data storage decisions (SQL vs NoSQL, specific database choice)
- Authentication/authorization approaches
- API design paradigms (REST, GraphQL, gRPC)

### Technology/Library Selections with Long-Term Impact
- Testing framework (NUnit, Moq, Testcontainers)
- Build tool (Dotnet, MSBuild, Cake)
- Infrastructure choices (Azure, GCP, Docker, self-hosted)

### Pattern Decisions Affecting Multiple Modules
- Error handling strategy across the application
- Logging/observability approach
- Code organization patterns
- Validation approach (where, how, what library)

### Performance vs Maintainability Trade-offs
- Caching strategy
- Optimization decisions with complexity cost
- Build-time vs runtime trade-offs

### Security Architecture Decisions
- Token storage approach
- Encryption strategy
- Security headers policy

## DON'T Create ADRs For

### Trivial Implementation Choices
- Variable naming
- Function parameter order
- File naming conventions

### Temporary Workarounds
- Short-term fixes
- Spike/experiment code
- Proof of concepts

### Standard Patterns from CLAUDE.md
- Using factory functions (already documented)
- Immutability (already a rule)
- TDD process (already required)

### Implementation Details with No Alternatives
- Straightforward code choices
- Only one obvious way to implement
- No trade-offs to discuss

### Decisions That Will Change Frequently
- UI component styling
- Copy/text content
- Feature flags (unless the flag system itself)

## Patterns That Indicate ADR Needed

| Pattern | Example |
|---------|---------|
| Multiple options discussed | "Should we use Shouldly or FluentAssertions?" |
| Trade-offs mentioned | "BullMQ is more complex but more robust" |
| "Why did we...?" questions | "Why PostgreSQL over MongoDB?" |
| Foundational decisions | "We're going with a monorepo structure" |

## Patterns That Indicate NO ADR Needed

| Pattern | Better Alternative |
|---------|-------------------|
| Code style choice | Document in CLAUDE.md |
| Already in guidelines | Reference existing docs |
| No alternatives exist | Not really a decision |
| Will change next week | Too volatile for ADR |