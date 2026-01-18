---
name: use-case-data-patterns
description: >
  Analyzes how user-facing use cases map to data access patterns and architecture. Auto-triggers
  when: implementing new features that need existing pattern understanding, explaining how a
  feature works in the data layer, planning changes affecting data access, asking "how does X
  work in our codebase?", or tracing a user action through the system architecture.
allowed-tools:
  - Read
  - Grep
  - Glob
---

# Use Case to Data Patterns Analyzer

> **Attribution**: Adapted from [Kieran O'Hara's dotfiles](https://github.com/kieran-ohara/dotfiles).

Maps user-facing use cases to underlying data access patterns, database interactions, and architectural implementation.

**Core Function:** Create analytical reports that trace use cases through the architecture. This is purely analytical - no file edits, no implementations.

## Analysis Process

### 1. Parse the Use Case
Extract the core user action, expected behavior, and business requirements.

### 2. Trace Through Architecture Layers
- Entry points (controllers, handlers, routes)
- Middleware, guards, interceptors
- Business logic (services, use cases, domain models)
- Abstractions and implementations
- Database tables, models, schemas, migrations
- Caching strategies

### 3. Map Data Access Patterns
- Data flow from request to storage/retrieval
- Database queries (ORM, raw SQL, query builders)
- Data transformations, mappers, DTOs
- Caching layers and strategies
- External API calls, integrations

### 4. Analyze Architectural Patterns
- Design patterns used (repository, factory, strategy, adapter)
- How code follows project's architectural principles
- Version-specific or conditional implementations
- Abstraction separations

### 5. Identify Gaps
- Missing data access patterns
- Incomplete implementations
- Architectural improvements needed
- Scalability/performance concerns

## Report Structure

See @report-template.md for the full report format.

**Quick structure:**
```
# Use Case Analysis Report
## Use Case Summary
## Architecture Flow
## Data Access Patterns
## Relevant Code Locations
## Current Implementation Status
## Gaps and Missing Patterns
## Recommendations
```

## Key Principles

1. **Be Specific** - Reference exact file paths, function names, class names
2. **Follow the Code** - Actually trace through codebase, don't assume
3. **Consider All Implementations** - Analyze patterns across implementations
4. **Respect Project Structure** - Follow the codebase's conventions
5. **Database Focus** - Pay attention to schemas, migrations, queries, ORM
6. **Version Awareness** - Note version-specific implementations
7. **Test Alignment** - Consider testing approach

## What NOT To Do

- ❌ Edit any files
- ❌ Create documentation files
- ❌ Implement code changes
- ❌ Suggest specific code implementations
- ❌ Make assumptions about unexamined code

## Handling Uncertainty

If you cannot find specific patterns:
- State what you searched for and where
- Explain what you expected based on architecture
- Note as a gap in recommendations
- Suggest where patterns would logically belong

## Reference Files

- @report-template.md - Complete report structure with all sections

## Your Value

Provide deep, accurate architectural analysis that helps understand how use cases map to actual implementation. Be thorough, be specific, and clearly distinguish between what exists and what's missing.