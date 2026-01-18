# Documentation Format

Templates for CLAUDE.md entries.

## For Principles/Guidelines

```markdown
### New Principle Name

Brief explanation of why this matters.

**Key points:**
- Specific guideline with clear rationale
- Another guideline with example
- Edge case or gotcha to watch for

```csharp
// ✅ GOOD - Example following the principle
var example = "demonstrating correct approach";

// ❌ BAD - Example showing what not to do
var bad = "demonstrating wrong approach";
```
```

## For Gotchas/Edge Cases

```markdown
#### Gotcha: Descriptive Title

**Context**: When does this occur
**Issue**: What goes wrong
**Solution**: How to handle it

```csharp
// ✅ CORRECT - Solution example
var correct = HandleEdgeCase();

// ❌ WRONG - What causes the problem
var wrong = NaiveApproach();
```
```

## For Project-Specific Knowledge

```markdown
## Project Setup / Architecture / Domain Knowledge

### Specific Area

Clear explanation with:
- Why this is important
- How it affects development
- Examples where relevant
```

## Documentation Proposal Template

```
## CLAUDE.md Learning Integration

### Summary
Brief description of what was learned and why it matters.

### Proposed Location
**Section**: [Section Name]
**Position**: [Before/After existing content, or new section]

### Proposed Addition

```markdown
[Exact markdown content to add to CLAUDE.md]
```

### Rationale
- Why this learning is valuable
- How it fits with existing guidelines
- What problems it helps prevent

### Verification Checklist
- [ ] Learning is not already documented
- [ ] Fits naturally into CLAUDE.md structure
- [ ] Maintains consistent voice and style
- [ ] Includes concrete examples if applicable
- [ ] Prevents future confusion or wasted time
```

## Voice and Style

- **Imperative tone**: "Use X", "Avoid Y", "Always Z"
- **Clear rationale**: Explain WHY, not just WHAT
- **Concrete examples**: Show good and bad patterns
- **Emphasis markers**: Use **bold** for critical points, ❌ ✅ for patterns
- **Structured format**: Headings, bullet points, code blocks

## CLAUDE.md Sections

Where learnings typically fit:

| Section | Content Type |
|---------|-------------|
| Core Philosophy | Fundamental principles |
| Testing Principles | Test strategy and patterns |
| Architecture | Layering, separation of concerns |
| Code Style | Naming, structure, immutability |
| Development Workflow | TDD process, refactoring, commits |
| Domain-Driven Design | Ubiquitous language, bounded contexts |
| Working with Claude | Expectations and communication |

Create new sections for:
- Project-specific setup
- Domain-specific knowledge
- Tool configurations
- Performance considerations
- Security patterns