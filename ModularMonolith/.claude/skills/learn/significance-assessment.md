# Significance Assessment

Detailed criteria for deciding what to document.

## Document If ANY Are True

- ✅ Would save future developers significant time (>30 minutes)
- ✅ Prevents a class of bugs or errors
- ✅ Reveals non-obvious behavior or constraints
- ✅ Captures architectural rationale or trade-offs
- ✅ Documents domain-specific knowledge
- ✅ Identifies effective patterns or anti-patterns
- ✅ Clarifies tool setup or configuration gotchas

## Skip If ALL Are True

- ❌ Already well-documented in CLAUDE.md
- ❌ Obvious or standard practice
- ❌ Trivial change (typos, formatting)
- ❌ Implementation detail unlikely to recur

## Quality Standards

Before adding to CLAUDE.md:

| Check | Question |
|-------|----------|
| Actionable | Does the reader know exactly what to do? |
| Specific | Does it avoid vague guidelines? |
| Justified | Is the reasoning explained? |
| Discoverable | Are headings and keywords clear? |
| Consistent | Does it match existing conventions? |

## Duplication Check

Before proposing additions:

1. Search CLAUDE.md for related keywords
2. Check if principle is implied by existing guidelines
3. Verify this adds new, non-obvious information
4. Consider updating existing section vs adding new one

```bash
# Search for related content
grep -i "pattern" CLAUDE.md
```

## Examples of Worth Documenting

| Trigger | Why Document |
|---------|-------------|
| "This took me 2 hours to figure out" | Saves future time |
| "The error message was misleading" | Prevents confusion |
| "I assumed X but it's actually Y" | Corrects misconceptions |
| "This pattern worked really well" | Shares effective approaches |
| "Never do X because..." | Prevents known mistakes |

## Examples NOT Worth Documenting

| Trigger | Why Skip |
|---------|----------|
| "Fixed a typo" | Trivial |
| "Standard NUnit setup" | Common knowledge |
| "This specific bug in this file" | Too narrow |
| "Everyone knows you should..." | Already obvious |