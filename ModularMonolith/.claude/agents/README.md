# Claude Code Agents & Skills

This directory contains agents that require explicit invocation for fresh-context critique. For auto-triggering documentation and analysis, see the `skills/` directory.

## Hybrid Approach

We use a hybrid system optimised for different task types:

| Type | Location | Invocation | Best For |
|------|----------|------------|----------|
| **Agents** | `.claude/agents/` | Explicit (`/agent-name`) | Fresh-context critique |
| **Skills** | `.claude/skills/` | Auto-trigger | Documentation with conversation context |

**Why?** Agents spawn with fresh context, making them more objective when critiquing code you just wrote. Skills have full conversation context, which helps with documentation tasks.

## Agents (This Directory)

### `tdd-guardian`

**Purpose**: Ensures strict Test-Driven Development compliance.

**Use proactively when**:
- Planning to implement a new feature
- About to write any production code

**Use reactively when**:
- Code has been written (verify TDD was followed)
- Tests are green (assess refactoring opportunities)

**Core responsibility**: Enforce RED-GREEN-REFACTOR cycle, verify tests written first.

**Why an agent?** Fresh context provides objective critique of whether TDD was actually followed, without bias from having written the code.

---

### `refactor-scan`

**Purpose**: Assesses refactoring opportunities after tests pass (TDD's third step).

**Use proactively when**:
- Tests just turned green
- Considering creating abstractions
- Planning code improvements

**Use reactively when**:
- Noticing code duplication
- Reviewing code quality
- Evaluating semantic vs structural similarity

**Core responsibility**: Identify valuable refactoring (only refactor if adds value), distinguish knowledge duplication from structural similarity.

**Why an agent?** Fresh context enables honest assessment of whether code needs refactoring, without justifying decisions already made.

---

### `wip-guardian`

**Purpose**: Maintains living plan document for work in progress.

**Use proactively when**:
- Starting significant multi-step work
- Beginning feature requiring multiple PRs
- Starting complex refactoring or investigation

**Use reactively when**:
- Completing a step in the plan
- Learning something that changes the plan
- Encountering blockers
- End of work session (checkpoint)
- Before creating PR (verify completion)

**Core responsibility**:
- Create and maintain temporary `WIP.md` file
- Enforce small PRs, incremental work, tests passing
- Coordinate agents and skills at appropriate times
- **DELETE `WIP.md` when complete**

---

## Skills (See `.claude/skills/`)

These auto-trigger based on conversation context:

| Skill | Auto-triggers When | Purpose |
|-------|-------------------|---------|
| `learn` | Discovering gotchas, completing features, fixing bugs | Captures learnings → CLAUDE.md |
| `adr` | Discussing architecture trade-offs, technology choices | Creates Architecture Decision Records |
| `use-case-data-patterns` | Asking "how does X work?", analyzing data flows | Maps use cases to data patterns |

## Workflow Integration

```
1. Start significant work
   └─→ Invoke wip-guardian: Creates WIP.md

2. For each step in plan
   └─→ Invoke tdd-guardian: Verify TDD (RED)
   └─→ Write minimal code (GREEN)
   └─→ Invoke refactor-scan: Assess improvements (REFACTOR)
   └─→ Invoke wip-guardian: Update progress

3. When architectural decision arises
   └─→ Invoke wip-guardian: Document decision point
   └─→ adr skill auto-triggers

4. When learning occurs
   └─→ Invoke wip-guardian: Update plan
   └─→ learn skill auto-triggers

5. Feature complete
   └─→ learn skill captures final learnings
   └─→ Invoke wip-guardian: DELETE WIP.md
```

## Key Distinctions

### Agents vs Skills

| Aspect | Agents | Skills |
|--------|--------|--------|
| **Context** | Fresh (no conversation history) | Full conversation context |
| **Invocation** | Explicit | Auto-trigger |
| **Best for** | Objective critique | Documentation |
| **Examples** | tdd-guardian, refactor-scan | learn, adr |

### Documentation Types

| Tool | Lifespan | Purpose | Output |
|------|----------|---------|--------|
| `wip-guardian` | Temporary | Track progress | `WIP.md` (deleted when done) |
| `adr` skill | Permanent | Explain "why" decisions | `docs/adr/*.md` |
| `learn` skill | Permanent | Explain "how" to work | `CLAUDE.md` entries |

## Summary

- **Agents** (invoke explicitly): `tdd-guardian`, `refactor-scan`, `wip-guardian`
- **Skills** (auto-trigger): `learn`, `adr`, `use-case-data-patterns`

Agents provide fresh-context critique. Skills provide context-aware documentation.