# ADR-005: Compile-Time Architecture Enforcement via Roslyn Analyzers

**Date**: 2026-01-10

**Tags**: architecture, roslyn, analyzers, compile-time, testing, hexagonal-architecture, module-boundaries

## Context

In a modular monolith with hexagonal architecture, enforcing layer dependencies is critical to maintaining module isolation and preventing architectural erosion. Without enforcement, developers can inadvertently create dependencies that violate architectural rules (e.g., Domain layer referencing Infrastructure).

**The Problem:**

We need a mechanism to enforce architectural rules such as:
- Domain layer can only reference shared Domain
- Application layer can reference Domain and shared Application
- Infrastructure layer can reference Domain and Application (dependency inversion)
- Controllers layer can reference Application but not Infrastructure directly
- No circular dependencies between layers
- No cross-module direct dependencies (only through Messages projects)

**Previous Approach: Runtime Architecture Tests**

We initially used NUnit-based architecture tests in `Testing.Architecture.Events`:
```csharp
[Test]
public void Domain_layer_should_only_reference_shared_domain()
{
    // Use reflection to inspect assembly references at test time
    var domainAssembly = typeof(Event).Assembly;
    var references = domainAssembly.GetReferencedAssemblies();

    references.Should().OnlyContain(r => r.Name == "Domain");
}
```

**Limitations of Runtime Tests:**
1. **Late feedback** - Violations discovered only when tests run (after code is written)
2. **No IDE integration** - Developers get no warning while coding
3. **Build dependency** - Must run tests to catch violations
4. **Separate from compilation** - Tests can be skipped, CI can fail after merge
5. **Slower feedback loop** - RED-GREEN-REFACTOR cycle interrupted by test runs

**Question:** How do we enforce architectural boundaries with immediate, actionable feedback?

**Requirements:**
- Catch architectural violations at compile time (fail-fast)
- Provide IDE warnings/errors as developers type
- Integrate with standard build process (dotnet build)
- Support all modules with consistent pattern
- Allow shared enforcement logic across modules
- Maintainable and extensible for new rules

## Decision

We will use **Roslyn analyzers** packaged as analyzer projects to enforce architectural rules at compile time.

**Pattern:**

Each module has an `Architecture.{Module}` analyzer project (e.g., `Architecture.Events`) that:
1. Implements `DiagnosticAnalyzer` to inspect compilation references
2. Defines layer-specific rules (Domain, Application, Infrastructure, etc.)
3. Reports violations as compiler errors during build
4. Integrates with IDEs for real-time feedback

**Shared Infrastructure:**

Common enforcement logic will be extracted to `Architecture.Common` containing:
- `ILayerRule` interface
- `LayerRuleBase` abstract class
- `AssemblyReferenceReader` utility

**Diagnostic ID Convention:**

Diagnostic IDs follow the pattern: `ARCH_{MODULE}_{NNN}`
- Events module: `ARCH_EVENT_001`, `ARCH_EVENT_002`, etc.
- Tickets module: `ARCH_TICKETS_001`, `ARCH_TICKETS_002`, etc.
- Global rules: `ARCH_001`, `ARCH_002`, etc. (no module prefix)

**Implementation Example (Events Module):**

```csharp
// Architecture.Events/ProjectDependencies/ProjectDependenciesGuardian.cs
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProjectDependenciesGuardian : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<ILayerRule> LayerRules =
    [
        new DomainLayerRule(),
        new ApplicationLayerRule(),
        new ControllersLayerRule(),
        new InfrastructureLayerRule(),
        new MessagesLayerRule(),
        new MessagingLayerRule()
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var currentAssembly = context.Compilation.AssemblyName;
        var referencedAssemblies = AssemblyReferenceReader.GetReferencedProjectAssemblies(context.Compilation);

        foreach (var rule in LayerRules.Where(r => r.AppliesTo(currentAssembly)))
        {
            rule.Validate(context, currentAssembly, referencedAssemblies);
        }
    }
}

// Architecture.Events/ProjectDependencies/Rules/DomainLayerRule.cs
internal sealed class DomainLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_EVENT_001",
        title: "Invalid Domain layer reference",
        messageFormat: "{0} cannot reference {1}. Domain layer may only reference shared Domain.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.DomainEvents;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.SharedDomain
    ];
}
```

**Project File Configuration:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <IsPackable>true</IsPackable>
        <IncludeBuildOutput>false</IncludeBuildOutput>
        <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
    </PropertyGroup>

    <ItemGroup>
        <None Include="$(OutputPath)\$(AssemblyName).dll"
              Pack="true"
              PackagePath="analyzers/dotnet/cs"
              Visible="false" />
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" />
        <PackageReference Include="Microsoft.CodeAnalysis.CSharp" />
    </ItemGroup>
</Project>
```

**Consuming the Analyzer:**

Projects reference the analyzer via `ProjectReference`:

```xml
<!-- Domain.Events/Domain.Events.csproj -->
<ItemGroup>
    <ProjectReference Include="..\Architecture.Events\Architecture.Events.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false" />
</ItemGroup>
```

## Alternatives Considered

### Alternative 1: Continue with Runtime Architecture Tests (NUnit + Reflection)

**Approach:**
```csharp
[TestFixture]
public class ModuleArchitectureTests
{
    [Test]
    public void Domain_should_not_reference_infrastructure()
    {
        var domainAssembly = typeof(Event).Assembly;
        var references = domainAssembly.GetReferencedAssemblies();

        references.Should().NotContain(r => r.Name.Contains("Infrastructure"));
    }
}
```

**Pros:**
- Familiar testing approach (NUnit)
- Already implemented for Events module
- Easy to write and understand
- Can use assertion libraries (Shouldly, FluentAssertions)
- Flexible - can check any aspect of assembly structure

**Cons:**
- **Late feedback** - Violations discovered after code is written
- **No IDE integration** - Developers don't see violations while coding
- **Can be skipped** - CI can be bypassed, tests can be commented out
- **Slower feedback** - Must run tests to catch violations
- **Separate step** - Not part of standard compilation
- **Interrupts flow** - Breaks TDD cycle with architecture failures

**Why Rejected:**

The feedback loop is too slow. Developers may write code violating architectural rules, commit it, and only discover the violation during CI build. This creates rework and slows development. Additionally, runtime tests can be skipped (intentionally or accidentally), allowing violations to slip through.

The fundamental issue is that architectural violations are **build-time errors**, not **runtime errors**. They should be treated as such - caught by the compiler, not by tests.

---

### Alternative 2: Manual Code Reviews

**Approach:**
- Rely on PR reviews to catch architectural violations
- Document rules in `Documents/Architecture.md`
- Trust developers to follow guidelines

**Pros:**
- No tooling needed
- Flexible - humans can reason about context
- Encourages architectural discussions in PRs
- Low implementation effort

**Cons:**
- **Unreliable** - Easy to miss violations in complex PRs
- **Inconsistent** - Different reviewers may interpret rules differently
- **Slow feedback** - Only discover violations during PR review
- **Error-prone** - Manual process, subject to human error
- **Doesn't scale** - As team grows, harder to maintain consistency
- **No enforcement** - Can be bypassed or ignored

**Why Rejected:**

Humans are fallible. Reviewers may miss violations, especially in large PRs with many files. Architectural rules should be **enforced automatically**, not relied upon through manual review. Code reviews should focus on business logic, design decisions, and maintainability - not catching basic architectural violations.

---

### Alternative 3: Third-Party Architecture Testing Libraries (NetArchTest, ArchUnitNET)

**Approach:**
```csharp
[Test]
public void Domain_should_not_depend_on_infrastructure()
{
    Types.InAssembly(typeof(Event).Assembly)
        .That().ResideInNamespace("Domain.Events")
        .Should().NotHaveDependencyOn("Infrastructure.Events")
        .GetResult().IsSuccessful.Should().BeTrue();
}
```

**Pros:**
- More expressive than raw reflection
- Established libraries with good APIs
- Can test complex rules (namespace dependencies, inheritance hierarchies)
- Good documentation and examples
- Active maintenance

**Cons:**
- **Still runtime tests** - Same late feedback problem as Alternative 1
- **No IDE integration** - No warnings while coding
- **External dependency** - Adds third-party library to testing stack
- **Learning curve** - Team must learn library API
- **Abstraction overhead** - Another layer between problem and solution
- **Same fundamental issue** - Architecture violations discovered during test runs, not compilation

**Why Rejected:**

While these libraries provide better APIs than raw reflection, they don't solve the fundamental problem: **late feedback**. Architecture violations should be caught during compilation, not during testing. These libraries are optimized for runtime validation, which is the wrong point in the feedback loop.

Additionally, introducing a third-party library adds dependency risk and learning overhead without solving the core issue.

---

### Alternative 4: Build Scripts with Custom Tooling

**Approach:**
- Write custom PowerShell/Bash scripts to analyze `.csproj` files
- Run as pre-build step or CI check
- Parse project references and validate against rules

```powershell
# Validate-Architecture.ps1
$domainCsproj = [xml](Get-Content "Domain.Events/Domain.Events.csproj")
$references = $domainCsproj.Project.ItemGroup.ProjectReference.Include

if ($references -match "Infrastructure") {
    Write-Error "Domain cannot reference Infrastructure"
    exit 1
}
```

**Pros:**
- Simple to implement initially
- No framework dependencies
- Fast execution
- Can run in CI/CD pipeline
- Full control over logic

**Cons:**
- **Fragile** - Breaks if project file structure changes
- **No IDE integration** - Developers don't see violations in real-time
- **Maintenance burden** - Scripts must be maintained separately
- **Limited analysis** - Hard to analyze indirect dependencies
- **Not cross-platform** - PowerShell/Bash differences
- **No semantic analysis** - Only inspects XML, can't analyze actual code
- **Error-prone** - Custom parsing is unreliable

**Why Rejected:**

Custom scripts are brittle and hard to maintain. They can't analyze semantic relationships (e.g., which assemblies are actually referenced by compiled code vs project files). More importantly, they still don't provide IDE integration or compile-time feedback.

Roslyn analyzers are the **standard, supported way** to perform compile-time analysis in .NET. Building custom tooling when a proper solution exists is unnecessary complexity.

---

### Alternative 5: Hybrid Approach (Runtime Tests + Pre-Commit Hooks)

**Approach:**
- Keep NUnit architecture tests
- Add git pre-commit hook to run tests before commit
- Fail commit if tests fail

```bash
# .git/hooks/pre-commit
dotnet test Testing.Architecture.Events || exit 1
```

**Pros:**
- Catches violations before code is committed
- Earlier feedback than CI-only approach
- Leverages existing tests
- Easy to implement

**Cons:**
- **Still no IDE integration** - Developers don't see violations while coding
- **Slow pre-commit** - Running tests before every commit slows workflow
- **Can be bypassed** - `git commit --no-verify` skips hooks
- **Local-only** - Not enforced in CI unless separately configured
- **Doesn't scale** - As tests grow, pre-commit gets slower
- **Interrupts flow** - Breaks commit workflow with test runs

**Why Rejected:**

Pre-commit hooks improve feedback timing but still don't solve the core issue: **developers need to see violations as they type**, not when they commit. Additionally, pre-commit hooks can be easily bypassed and don't integrate with the IDE.

Roslyn analyzers provide real-time feedback in the IDE with squiggly lines and error messages - the **optimal point** in the feedback loop.

---

## Consequences

### Positive

1. **Immediate Feedback (Fail-Fast)**
   - Violations appear as compiler errors during build
   - No need to wait for test runs
   - Impossible to merge code with violations (build fails)
   - Developers see violations **as they type** in IDE

2. **IDE Integration**
   - Visual Studio, Rider, VS Code show violations with squiggly lines
   - Hover over violation for detailed error message
   - IntelliSense prevents invalid references
   - Real-time feedback shortens iteration cycles

3. **Better Developer Experience**
   - No context switching to test output
   - Violations appear where code is written (in the editor)
   - Error messages clearly explain what's wrong and why
   - Can't accidentally skip enforcement (unlike tests)

4. **Consistent Enforcement**
   - Runs on every build (local and CI)
   - Cannot be bypassed or disabled
   - Applies to all developers uniformly
   - Integrated with standard tooling (dotnet build)

5. **Faster TDD Cycles**
   - Architecture violations caught immediately (compile time)
   - Tests focus on business behavior, not architectural structure
   - No need for separate architecture test runs
   - RED-GREEN-REFACTOR flow uninterrupted by architecture checks

6. **Shared Logic (Architecture.Common)**
   - Common rules (`ILayerRule`, `LayerRuleBase`) reusable across modules
   - Consistent patterns for all modules
   - Centralized enforcement logic
   - Easy to add new modules following established pattern

7. **Maintainable and Extensible**
   - Adding new rules is straightforward (create new `ILayerRule` implementation)
   - Removing old tests reduces maintenance burden
   - Clear separation: analyzers enforce structure, tests verify behavior
   - Standard Roslyn API (well-documented, widely used)

8. **Self-Documenting Architecture**
   - Rules in code (not separate documentation)
   - Error messages explain architectural constraints
   - New developers learn architecture from compiler feedback
   - No ambiguity about what's allowed vs forbidden

### Negative

1. **Initial Implementation Effort**
   - Each module needs `Architecture.{Module}` project
   - Must write analyzer code (more complex than simple tests)
   - Learning curve for Roslyn analyzer API
   - Migration effort from existing runtime tests

   **Mitigation**: After implementing first analyzer (Events), pattern is established. New modules copy the pattern with minimal changes. Architecture.Common will reduce duplication further.

2. **Debugging Complexity**
   - Analyzers run during compilation (harder to debug than tests)
   - Roslyn API is complex (learning curve for team)
   - Errors in analyzer code can break builds unexpectedly

   **Mitigation**:
   - Use `EnforceExtendedAnalyzerRules` to catch analyzer errors early
   - Test analyzers in isolation before deploying
   - Document common patterns in Architecture.Common
   - Once working, analyzers rarely need changes

3. **Build-Time Overhead**
   - Analyzers run on every compilation (small performance cost)
   - Complex rules may slow down builds
   - More projects in solution (one analyzer per module)

   **Mitigation**: Analyzers are highly optimized (Roslyn team prioritizes performance). In practice, the overhead is negligible (milliseconds). The benefit of immediate feedback far outweighs minor build slowdown.

4. **Requires Understanding of Compilation Model**
   - Developers may need to understand how analyzers work
   - More moving parts in build process
   - Potential confusion about analyzer vs project references

   **Mitigation**: Document in `Documents/Architecture.md` how analyzers work. In practice, developers just see compiler errors - they don't need to understand Roslyn internals.

5. **Testing Analyzers Themselves**
   - Analyzers need tests to verify they work correctly
   - More complex than testing normal code
   - Requires Roslyn testing infrastructure

   **Mitigation**: Roslyn provides `Microsoft.CodeAnalysis.Testing` for analyzer tests. Once Architecture.Common is established, new rules follow proven patterns with minimal testing overhead.

### Neutral

1. **Shift from Test-Time to Compile-Time**
   - Philosophical shift: architecture rules are build constraints, not runtime tests
   - Tests now focus purely on behavior (as they should)
   - Clearer separation of concerns

2. **Diagnostic ID Management**
   - Need to track diagnostic IDs across modules
   - Potential for ID collisions if not careful
   - Convention (`ARCH_{MODULE}_{NNN}`) mitigates this

3. **Integration with CI/CD**
   - No change - `dotnet build` still runs in CI
   - Violations still fail build (now earlier in pipeline)
   - Removes need for separate architecture test step

## Implementation Notes

### Applying This Pattern to New Modules

When creating a new module (e.g., `Payments`), follow these steps:

**1. Create Analyzer Project:**

```bash
dotnet new classlib -n Architecture.Payments
cd Architecture.Payments
```

**2. Configure Project File:**

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFramework>net8.0</TargetFramework>
        <IsPackable>true</IsPackable>
        <IncludeBuildOutput>false</IncludeBuildOutput>
        <EnforceExtendedAnalyzerRules>true</EnforceExtendedAnalyzerRules>
    </PropertyGroup>

    <ItemGroup>
        <None Include="$(OutputPath)\$(AssemblyName).dll"
              Pack="true"
              PackagePath="analyzers/dotnet/cs" />
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.CodeAnalysis.Analyzers" />
        <PackageReference Include="Microsoft.CodeAnalysis.CSharp" />
    </ItemGroup>
</Project>
```

**3. Define Known Assemblies:**

```csharp
// ProjectDependencies/KnownAssemblies.cs
internal static class KnownAssemblies
{
    internal const string DomainPayments = "Domain.Payments";
    internal const string ApplicationPayments = "Application.Payments";
    // ... etc

    internal const string SharedDomain = "Domain";
    internal const string SharedApplication = "Application";
    internal const string SharedInfrastructure = "Infrastructure";

    internal static bool IsKnownAssembly(string name) { /* ... */ }
}
```

**4. Implement Layer Rules:**

Copy rule templates from `Architecture.Events`, updating:
- Diagnostic IDs: `ARCH_PAYMENTS_001`, `ARCH_PAYMENTS_002`, etc.
- Target assemblies: `Domain.Payments`, `Application.Payments`, etc.
- Error messages: Reference Payments module

**5. Create ProjectDependenciesGuardian:**

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProjectDependenciesGuardian : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<ILayerRule> LayerRules =
    [
        new DomainLayerRule(),
        new ApplicationLayerRule(),
        // ... all layer rules
    ];

    // ... same implementation as Events
}
```

**6. Reference Analyzer from Projects:**

```xml
<!-- Domain.Payments/Domain.Payments.csproj -->
<ItemGroup>
    <ProjectReference Include="..\Architecture.Payments\Architecture.Payments.csproj"
                      OutputItemType="Analyzer"
                      ReferenceOutputAssembly="false" />
</ItemGroup>
```

### Diagnostic ID Naming Convention

**Format:** `ARCH_{MODULE}_{NNN}`

**Examples:**
- `ARCH_EVENT_001` - Events module, Domain layer rule
- `ARCH_EVENT_002` - Events module, Application layer rule
- `ARCH_TICKETS_001` - Tickets module, Domain layer rule
- `ARCH_001` - Global rule (no module prefix)

**Numbering:**
- Start at 001 for each module
- Increment sequentially as new rules added
- Keep a mapping in each `ProjectDependenciesGuardian.cs`:

```csharp
// Diagnostic IDs for Events module:
// ARCH_EVENT_001 - Domain layer dependencies
// ARCH_EVENT_002 - Application layer dependencies
// ARCH_EVENT_003 - Controllers layer dependencies
// ARCH_EVENT_004 - Infrastructure layer dependencies
// ARCH_EVENT_005 - Messages layer dependencies
// ARCH_EVENT_006 - Messaging layer dependencies
```

### Migration from Runtime Tests

**Events module migration (completed):**
1. Created `Architecture.Events` project
2. Implemented `ProjectDependenciesGuardian` with layer rules
3. Removed `Testing.Architecture.Events` project
4. Deleted runtime tests (`Module.specs.cs`, `Module.steps.cs`)
5. Updated all Events layer projects to reference analyzer

**Future module migrations:**
1. Tickets module - follow Events pattern
2. Keycloak.Users module - follow Events pattern
3. Any new modules - use analyzer from start

### Architecture.Common Extraction (Future Work)

When second module (Tickets) implements analyzers, extract shared code:

**Shared Components:**
- `ILayerRule` interface
- `LayerRuleBase` abstract class
- `AssemblyReferenceReader` utility

**Module-Specific Components:**
- `KnownAssemblies` (each module has different assembly names)
- `ProjectDependenciesGuardian` (each module may have different rules)
- Concrete layer rules (rules may vary by module)

### Real Example: Catching a Violation

**Scenario:** Developer tries to add Infrastructure reference to Domain layer

```csharp
// Domain.Events/Event/Event.cs
using Infrastructure.Events; // ❌ Violation!

public class Event : AggregateRoot
{
    // ...
}
```

**What Happens:**

1. **IDE shows squiggly line** under `using Infrastructure.Events`
2. **Hover message:**
   ```
   Error ARCH_EVENT_001: Domain.Events cannot reference Infrastructure.Events.
   Domain layer may only reference shared Domain.
   ```
3. **Build fails:**
   ```
   Domain.Events.csproj(1,1): error ARCH_EVENT_001: Domain.Events cannot reference Infrastructure.Events.
   Domain layer may only reference shared Domain.
   ```

**Developer fixes:**
- Remove invalid using statement
- Build succeeds immediately
- No need to run tests to verify fix

### When to Create New Rules

**Create a new rule IF:**
- New layer added to module (e.g., `Adapters` layer)
- Cross-cutting constraint needed (e.g., "no one can reference Test projects")
- Module-specific restriction (e.g., "Messages projects can't reference Application")

**DON'T create a rule if:**
- Already covered by existing rule
- Too granular (e.g., specific class dependencies - use tests for that)
- Business logic constraint (architecture rules are structural, not behavioral)

### Verifying Analyzer Works

**Manual verification:**
1. Add invalid reference to a project
2. Build should fail with specific error message
3. Remove invalid reference
4. Build should succeed

**Testing analyzer (future):**
```csharp
// Architecture.Events.Tests/DomainLayerRuleTests.cs
[Test]
public async Task DomainLayerRule_WhenInfrastructureReferenced_ReportsDiagnostic()
{
    var testCode = @"
        // Domain project referencing Infrastructure
        using Infrastructure.Events;
    ";

    var expected = new DiagnosticResult("ARCH_EVENT_001", DiagnosticSeverity.Error);
    await VerifyAnalyzer(testCode, expected);
}
```

## Related Decisions

- **ADR-001: Initial Architecture** - Establishes hexagonal architecture and layer boundaries
  - This ADR provides enforcement mechanism for those boundaries
  - Complements ADR-001's architectural principles with tooling

- **ADR-002: Initial Testing** - Describes testing strategy
  - This ADR shifts architecture validation from test-time to compile-time
  - Tests now focus purely on behavior (unit, integration, component)
  - Architecture enforcement separated from behavioral testing

- **ADR-003: Vertical Slicing Within Horizontal Layers** - Describes module organization
  - Analyzer projects live at module level: `Modules/{Module}/Architecture.{Module}/`
  - Follow same naming convention as other module projects

## References

- [Roslyn Analyzers Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/tutorials/how-to-write-csharp-analyzer-code-fix)
- [Microsoft.CodeAnalysis.Analyzers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.Analyzers)
- [DiagnosticAnalyzer Class](https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.diagnostics.diagnosticanalyzer)
- Implementation commit: `fc7410f` (Events module architecture enforcement)
- Events module implementation: `Modules/Events/Architecture.Events/`

---

## Summary

**Decision**: Use **Roslyn analyzers** for compile-time architecture enforcement instead of runtime NUnit tests.

**Key Principle**: Architectural violations are **build errors**, not **runtime errors**. Catch them at compile time with immediate IDE feedback.

**Pattern**: Each module has `Architecture.{Module}` project with layer rules. Shared logic extracted to `Architecture.Common`.

**Diagnostic IDs**: `ARCH_{MODULE}_{NNN}` (e.g., `ARCH_EVENT_001`, `ARCH_TICKETS_001`)

**Rationale**: Fail-fast feedback loop. Developers see violations as they type. Can't skip enforcement. Integrates with standard tooling. Better DX.

**Result**: Architecture violations impossible to commit. Tests focus on behavior. Faster development cycles. Self-documenting constraints.
