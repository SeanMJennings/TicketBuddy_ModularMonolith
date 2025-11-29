# Development Guidelines for Claude

## Core Philosophy

**TEST-DRIVEN DEVELOPMENT IS NON-NEGOTIABLE.** Every single line of production code must be written in response to a failing test. No exceptions. This is not a suggestion or a preference - it is the fundamental practice that enables all other principles in this document.

I follow Test-Driven Development (TDD) with a strong emphasis on behavior-driven testing and functional programming principles. All work should be done in small, incremental changes that maintain a working state throughout development.

## Quick Reference

**Key Principles:**

- Write tests first (TDD)
- Test behavior, not implementation
- Immutable data only
- Small, pure functions

**Preferred Tools:**

- **Language**: Dotnet C#
- **Testing**: NUnit + Moq + Testcontainers
- **State Management**: Prefer immutable patterns

## Testing Principles

**Core principle**: Test behavior, not implementation. 100% coverage through business behavior.

**Quick reference:**
- Write tests first (TDD non-negotiable)
- Test through public API exclusively
- Tests must document expected business behavior
- No 1:1 mapping between test files and implementation files

For comprehensive testing guidelines including:
- Behavior-driven testing principles and anti-patterns
- Test data patterns and factory functions with full examples
- Achieving 100% coverage through business behavior
- Testing tools (Dotnet test, NUnit, Moq)
- Validating test data with schemas

See @~/.claude/docs/testing.md

## Architecture

**Core principle**: Focus on architecture specializing in simplicity and clarity. Eliminate all sources of incidental complexity.

**Quick reference:**
- Apply hexagonal architecture making use of ports and adapters when needed.
- Avoid introducing unnecessary abstractions, layers, or dependencies.
- If a requirement is ambiguous, ask clarifying questions before proceeding.

For architecture guidelines including:
- Hexagonal architecture principles and patterns
- Eliminating incidental complexity
- Design review features

See @~/.claude/docs/architecture.md

## Code Style

**Core principle**: Functional programming with immutable data. Self-documenting code.

**Quick reference:**
- No data mutation - immutable data structures only
- Pure functions wherever possible
- No nested if/else - use early returns or composition
- No comments - code should be self-documenting
- Prefer options objects over positional parameters

For comprehensive code style guidelines including:
- OOP programming patterns and when to use heavy abstractions
- Code structure principles (max 2 levels nesting)
- Naming conventions (functions, types, constants, files)
- Self-documenting code patterns (no comments)
- Options objects pattern with examples

See @~/.claude/docs/code-style.md

## Development Workflow

**Core principle**: RED-GREEN-REFACTOR. TDD is the fundamental practice.

**Quick reference:**
- RED: Write failing test first (NO production code without failing test)
- GREEN: Write MINIMUM code to pass test
- REFACTOR: Assess improvement opportunities (only refactor if adds value)
- Always commit before refactoring
- Semantic abstraction (meaning) over structural similarity (appearance)
- DRY = Don't repeat knowledge, not code structure

For comprehensive workflow guidelines including:
- TDD process with quality gates
- Anti-patterns in tests to avoid
- Verifying TDD compliance via git history
- Complete TDD example workflow (RED-GREEN-REFACTOR)
- Refactoring: the critical third step
- Refactoring priority classification (Critical/High/Nice/Skip)
- Understanding DRY - knowledge vs code
- Semantic vs structural decision framework
- Commit guidelines and PR standards

See @~/.claude/docs/workflow.md

## Domain Driven Design

**Core principle**: Model the domain accurately using ubiquitous language and bounded contexts.

**Quick reference:**
- Use ubiquitous language consistently in code and tests
- Define bounded contexts for complex domains
- Focus on core domain logic, avoid over-engineering

For comprehensive DDD guidelines including:
- Ubiquitous language patterns
- Bounded context definitions and examples
- Aggregates, entities, and value objects
- Domain events and services

See @~/.claude/docs/ddd-summary.md

## Working with Claude

**Core principle**: Think deeply, follow TDD strictly, capture learnings while context is fresh.

**Quick reference:**
- ALWAYS FOLLOW TDD - no production code without failing test
- Assess refactoring after every green (but only if adds value)
- Update CLAUDE.md when introducing meaningful changes
- Ask "What do I wish I'd known at the start?" after significant changes
- Document gotchas, patterns, decisions, edge cases while context is fresh

For comprehensive guidance including:
- Complete expectations checklist
- Learning documentation framework (7 criteria for what to document)
- Types of learnings to capture (gotchas, patterns, anti-patterns, decisions)
- Documentation format templates
- Code change principles
- Communication guidelines

See @~/.claude/docs/working-with-claude.md

## Summary

The key is to write clean, testable, functional code that evolves through small, safe increments. Every change should be driven by a test that describes the desired behavior, and the implementation should be the simplest thing that makes that test pass. When in doubt, favor simplicity and readability over cleverness.
