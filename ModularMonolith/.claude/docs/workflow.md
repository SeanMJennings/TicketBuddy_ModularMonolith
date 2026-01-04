# Development Workflow

## TDD Process - THE FUNDAMENTAL PRACTICE

**CRITICAL**: TDD is not optional. Every feature, every bug fix, every change MUST follow this process:

Follow Red-Green-Refactor strictly:

1. **Red**: Write a failing test for the desired behavior. NO PRODUCTION CODE until you have a failing test.
2. **Green**: Write the MINIMUM code to make the test pass. Resist the urge to write more than needed.
3. **Refactor**: Assess the code for improvement opportunities. If refactoring would add value, clean up the code while keeping tests green. If the code is already clean and expressive, move on.

**Common TDD Violations to Avoid:**

- Writing production code without a failing test first
- Writing multiple tests before making the first one pass
- Writing more production code than needed to pass the current test
- Skipping the refactor assessment step when code could be improved
- Adding functionality "while you're there" without a test driving it

**Remember**: If you're typing production code and there isn't a failing test demanding that code, you're not doing TDD.

## TDD Quality Gates

Before allowing any commit, verify:
- ✅ All production code has a test that demanded it
- ✅ Tests verify behavior, not implementation
- ✅ Implementation is minimal (only what's needed)
- ✅ Refactoring assessment completed (if tests green)
- ✅ All tests pass

## Cross-Module Changes - Commitable Increments

**CRITICAL PATTERN**: When changes span multiple modules or bounded contexts, a commit must leave the ENTIRE system in a working state, not just the module being changed.

### The Problem

Breaking work into phases is valuable for PLANNING and UNDERSTANDING, but not necessarily for COMMITTING. When implementing cross-module changes:

- Completing one module (e.g., Events) doesn't mean the work is committable
- Dependent modules (e.g., Tickets) may fail when the first module changes
- Higher test tiers (Component, Acceptance) may fail even when Unit and Integration tests pass
- A commit must be atomic and leave the entire system working

### What "GREEN" Really Means

In TDD, GREEN means **all tests in the entire system pass**, not just:
- The module you're actively changing
- The new tests you just wrote
- Unit and Integration tests (Component and Acceptance must also pass)

```csharp
// RED-GREEN-REFACTOR with cross-module changes:

// ❌ WRONG - Not truly GREEN
// Events module: all tests passing ✓
// Tickets module: 15 tests failing ✗
// Component tests: 4 tests failing ✗
// Acceptance tests: 2 tests failing ✗
// → This is NOT GREEN, cannot commit

// ✅ CORRECT - Truly GREEN
// Events module: all tests passing ✓
// Tickets module: all tests passing ✓
// Component tests: all tests passing ✓
// Acceptance tests: all tests passing ✓
// → NOW you're GREEN, safe to commit
```

### Common Causes of Cross-Module Test Failures

1. **Entity Tracking Conflicts** (EF Core)
   - Problem: Query with tracking, then try to update same entity
   - Solution: Use `.AsNoTracking()` for existence checks before updates

2. **Test Data Inconsistencies**
   - Problem: Different test suites create entities with different properties
   - Solution: Use shared test data factories, align capacities/counts across suites

3. **Hardcoded Values**
   - Problem: Tests use hardcoded GUIDs instead of dynamic test data
   - Solution: Always use IDs from test setup, never hardcode entity references

4. **Domain Validation in Infrastructure**
   - Problem: EF Core parameterless constructors fail domain validation
   - Solution: Satisfy validation even though EF Core overwrites values

### Recommended Workflow for Cross-Module Changes

```bash
# 1. Plan phases/steps (good for organizing work)
# 2. Implement phase/step in primary module
# 3. Run FULL test suite (ALL modules, ALL test tiers)
dotnet test  # Not just the module you changed!

# 4. If tests fail in other modules/tiers:
#    a. Fix failures immediately (don't defer)
#    b. This is part of the same commit, not a separate fix
#    c. Dependent module failures = you're not GREEN yet

# 5. Only commit when ALL tests pass
git commit -m "feat: implement cross-module venue synchronization"

# 6. Repeat for next phase/step
```

### Example: Venue Aggregate Implementation

```csharp
// PHASE 1: Events module implements Venue aggregate
// - Unit tests: ✓ passing
// - Integration tests: ✓ passing
// → Can commit? NO - haven't checked dependent modules yet

// PHASE 2: Events module publishes VenueUpserted
// - Events tests: ✓ passing
// - Tickets tests: ✗ 15 failing (VenueRepository tracking issues)
// - Component tests: ✗ 4 failing (capacity mismatches)
// - Acceptance tests: ✗ 2 failing (hardcoded GUIDs)
// → Fix ALL these failures before committing

// After fixing dependent module failures:
// - Events tests: ✓ passing
// - Tickets tests: ✓ passing (AsNoTracking added)
// - Component tests: ✓ passing (capacity aligned)
// - Acceptance tests: ✓ passing (dynamic IDs used)
// → NOW can commit (all 107 tests passing)
```

### Key Insights

**Planning vs Committing:**
- Phases are for PLANNING work (organize thinking)
- Commits are for WORKING state (entire system functional)
- Sometimes one commit spans multiple planned phases
- This is correct - atomic commits are more important than matching plan structure

**Definition of GREEN:**
- GREEN = entire test suite passes
- Not just: module under change
- Not just: new tests
- Not just: lower test tiers (Unit, Integration)
- ALL tests, ALL modules, ALL tiers

**Time Saved by Following This:**
- Avoids uncommitable state (can't save work)
- Prevents "fix the tests" commits (breaks TDD narrative)
- Catches integration issues immediately (not later)
- Maintains continuous working state (can deploy anytime)

## Verifying TDD Compliance Retrospectively

To verify that code was developed test-first, examine git history:

```bash
# Check if test was written before implementation
git log -p --follow src/Domain/Orders/OrderProcessor.cs
git log -p --follow tests/Domain/Orders/OrderProcessorSpecs.cs

# Look for:
# ✅ GOOD: Test commit comes BEFORE implementation commit
# ❌ BAD: Implementation committed without corresponding test
# ❌ BAD: Test and implementation in same commit without clear RED phase
```

**During code review, check:**
- Was there a failing test before each production code change?
- Do commit messages indicate RED-GREEN-REFACTOR progression?
- Are refactoring commits separate from feature commits?

**Example good commit sequence:**
```
feat(test): add test for payment validation (RED)
feat: implement payment validation (GREEN)
refactor: extract payment validation constants (REFACTOR)
```

**Example bad commit sequence:**
```
feat: add payment validation with tests
```
(Combined test+implementation suggests test wasn't written first)

## TDD Example Workflow

```csharp
// Step 1: Red - Start with the simplest behavior
[TestFixture]
public partial class OrderProcessingSpecs : Specification
{
    [Test]
    public void calculates_total_with_shipping_cost()
    {
        Given(an_order_with_items_totaling, 30m);
        And(a_shipping_cost_of, 5.99m);
        When(processing_the_order);
        Then(the_total_should_be, 35.99m);
        And(the_shipping_cost_should_be, 5.99m);
    }
}

// Step 2: Green - Minimal implementation
public class OrderProcessor
{
    public ProcessedOrder Process(Order order)
    {
        var itemsTotal = order.Items
            .Select(item => item.Price * item.Quantity)
            .Sum();

        return new ProcessedOrder
        {
            Items = order.Items,
            ShippingCost = order.ShippingCost,
            Total = itemsTotal + order.ShippingCost
        };
    }
}

// Step 3: Red - Add test for free shipping behavior
[TestFixture]
public partial class OrderProcessingSpecs : Specification
{
    // ...existing test...

    [Test]
    public void applies_free_shipping_for_orders_over_50()
    {
        Given(an_order_with_items_totaling, 60m);
        And(a_shipping_cost_of, 5.99m);
        When(processing_the_order);
        Then(the_shipping_cost_should_be, 0m);
        And(the_total_should_be, 60m);
    }
}

// Step 4: Green - NOW we can add the conditional because both paths are tested
public class OrderProcessor
{
    public ProcessedOrder Process(Order order)
    {
        var itemsTotal = order.Items
            .Select(item => item.Price * item.Quantity)
            .Sum();

        var shippingCost = itemsTotal > 50 ? 0 : order.ShippingCost;

        return new ProcessedOrder
        {
            Items = order.Items,
            ShippingCost = shippingCost,
            Total = itemsTotal + shippingCost
        };
    }
}

// Step 5: Add edge case tests to ensure 100% behavior coverage
[TestFixture]
public partial class OrderProcessingSpecs : Specification
{
    // ...existing tests...

    [Test]
    public void charges_shipping_for_orders_exactly_at_50()
    {
        Given(an_order_with_items_totaling, 50m);
        And(a_shipping_cost_of, 5.99m);
        When(processing_the_order);
        Then(the_shipping_cost_should_be, 5.99m);
        And(the_total_should_be, 55.99m);
    }
}

// Step 6: Refactor - Extract constants and improve readability
public class OrderProcessor
{
    private const decimal FreeShippingThreshold = 50m;

    public ProcessedOrder Process(Order order)
    {
        var itemsTotal = CalculateItemsTotal(order.Items);
        var shippingCost = QualifiesForFreeShipping(itemsTotal)
            ? 0
            : order.ShippingCost;

        return new ProcessedOrder
        {
            Items = order.Items,
            ShippingCost = shippingCost,
            Total = itemsTotal + shippingCost
        };
    }

    private static decimal CalculateItemsTotal(IEnumerable<OrderItem> items)
    {
        return items.Select(item => item.Price * item.Quantity).Sum();
    }

    private static bool QualifiesForFreeShipping(decimal itemsTotal)
    {
        return itemsTotal > FreeShippingThreshold;
    }
};
```

## Refactoring - The Critical Third Step

Evaluating refactoring opportunities is not optional - it's the third step in the TDD cycle. After achieving a green state and committing your work, you MUST assess whether the code can be improved. However, only refactor if there's clear value - if the code is already clean and expresses intent well, move on to the next test.

### What is Refactoring?

Refactoring means changing the internal structure of code without changing its external behavior. The public API remains unchanged, all tests continue to pass, but the code becomes cleaner, more maintainable, or more efficient. Remember: only refactor when it genuinely improves the code - not all code needs refactoring.

### When to Refactor

- **Always assess after green**: Once tests pass, before moving to the next test, evaluate if refactoring would add value
- **When you see duplication**: But understand what duplication really means (see DRY below)
- **When names could be clearer**: Variable names, function names, or type names that don't clearly express intent
- **When structure could be simpler**: Complex conditional logic, deeply nested code, or long functions
- **When patterns emerge**: After implementing several similar features, useful abstractions may become apparent

**Remember**: Not all code needs refactoring. If the code is already clean, expressive, and well-structured, commit and move on. Refactoring should improve the code - don't change things just for the sake of change.

### Refactoring Priority Classification

Not all refactoring is equally valuable. Classify issues by severity:

**🔴 Critical (Fix Now Before Commit):**
- Immutability violations (mutations)
- Semantic knowledge duplication (same business rule in multiple places)
- Deeply nested code (>3 levels)
- Security issues or data leak risks

**⚠️ High Value (Should Fix This Session):**
- Unclear names affecting comprehension
- Magic numbers/strings used multiple times
- Long functions (>30 lines) with multiple responsibilities
- Missing constants for important business rules

**💡 Nice to Have (Consider Later):**
- Minor naming improvements
- Extraction of single-use helper functions
- Structural reorganization that doesn't improve clarity
- Cosmetic changes without functional benefit

**✅ Skip Refactoring:**
- Code that's already clean and expressive
- Structural similarity without semantic relationship
- Changes that would make code less clear
- Abstractions that aren't yet proven necessary

**Example Assessment:**

```csharp
// After achieving green:
public class OrderProcessor
{
    public ProcessedOrder Process(Order order)
    {
        var itemsTotal = order.Items.Select(item => item.Price).Sum();
        var shipping = itemsTotal > 50 ? 0 : 5.99m;
        return new ProcessedOrder 
        { 
            Items = order.Items, 
            Total = itemsTotal + shipping, 
            ShippingCost = shipping 
        };
    }
}

// ASSESSMENT:
// 🔴 Critical: None
// ⚠️ High Value:
//   - Magic number 50 (FREE_SHIPPING_THRESHOLD)
//   - Magic number 5.99 (STANDARD_SHIPPING_COST)
// 💡 Nice to Have:
//   - Could extract CalculateItemsTotal helper
// ✅ Skip:
//   - Overall structure is clear enough

// DECISION: Fix high-value issues (extract constants), skip nice-to-have
```

### Refactoring Guidelines

#### 1. Commit Before Refactoring

Always commit your working code before starting any refactoring. This gives you a safe point to return to:

```bash
git add .
git commit -m "feat: add payment validation"
# Now safe to refactor
```

#### 2. Look for Useful Abstractions Based on Semantic Meaning

Create abstractions only when code shares the same semantic meaning and purpose. Don't abstract based on structural similarity alone - **duplicate code is far cheaper than the wrong abstraction**.

```csharp
// Similar structure, DIFFERENT semantic meaning - DO NOT ABSTRACT
public bool ValidatePaymentAmount(decimal amount)
{
    return amount > 0 && amount <= 10000;
}

public bool ValidateTransferAmount(decimal amount)
{
    return amount > 0 && amount <= 10000;
}

// These might have the same structure today, but they represent different
// business concepts that will likely evolve independently.
// Payment limits might change based on fraud rules.
// Transfer limits might change based on account type.
// Abstracting them couples unrelated business rules.

// Similar structure, SAME semantic meaning - SAFE TO ABSTRACT
public string FormatUserDisplayName(string firstName, string lastName)
{
    return $"{firstName} {lastName}".Trim();
}

public string FormatCustomerDisplayName(string firstName, string lastName)
{
    return $"{firstName} {lastName}".Trim();
}

public string FormatEmployeeDisplayName(string firstName, string lastName)
{
    return $"{firstName} {lastName}".Trim();
}

// These all represent the same concept: "how we format a person's name for display"
// They share semantic meaning, not just structure
public string FormatPersonDisplayName(string firstName, string lastName)
{
    return $"{firstName} {lastName}".Trim();
}

// Replace all call sites throughout the codebase:
// Before:
// var userLabel = FormatUserDisplayName(user.FirstName, user.LastName);
// var customerName = FormatCustomerDisplayName(customer.FirstName, customer.LastName);
// var employeeTag = FormatEmployeeDisplayName(employee.FirstName, employee.LastName);

// After:
// var userLabel = FormatPersonDisplayName(user.FirstName, user.LastName);
// var customerName = FormatPersonDisplayName(customer.FirstName, customer.LastName);
// var employeeTag = FormatPersonDisplayName(employee.FirstName, employee.LastName);

// Then remove the original functions as they're no longer needed
```

#### 3. Understanding DRY - It's About Knowledge, Not Code

DRY (Don't Repeat Yourself) is about not duplicating **knowledge** in the system, not about eliminating all code that looks similar.

```csharp
// This is NOT a DRY violation - different knowledge despite similar code
public bool ValidateUserAge(int age)
{
    return age >= 18 && age <= 100;
}

public bool ValidateProductRating(int rating)
{
    return rating >= 1 && rating <= 5;
}

public bool ValidateYearsOfExperience(int years)
{
    return years >= 0 && years <= 50;
}

// These functions have similar structure (checking numeric ranges), but they
// represent completely different business rules:
// - User age has legal requirements (18+) and practical limits (100)
// - Product ratings follow a 1-5 star system
// - Years of experience starts at 0 with a reasonable upper bound
// Abstracting them would couple unrelated business concepts and make future
// changes harder. What if ratings change to 1-10? What if legal age changes?

// Another example of code that looks similar but represents different knowledge:
public string FormatUserDisplayName(User user)
{
    return $"{user.FirstName} {user.LastName}".Trim();
}

public string FormatAddressLine(Address address)
{
    return $"{address.Street} {address.Number}".Trim();
}

public string FormatCreditCardLabel(CreditCard card)
{
    return $"{card.Type} {card.LastFourDigits}".Trim();
}

// Despite the pattern "combine two strings with space and trim", these represent
// different domain concepts with different future evolution paths

// This IS a DRY violation - same knowledge in multiple places
public class Order
{
    public decimal CalculateTotal()
    {
        var itemsTotal = Items.Sum(item => item.Price);
        var shippingCost = itemsTotal > 50 ? 0 : 5.99m; // Knowledge duplicated!
        return itemsTotal + shippingCost;
    }
}

public class OrderSummary
{
    public decimal GetShippingCost(decimal itemsTotal)
    {
        return itemsTotal > 50 ? 0 : 5.99m; // Same knowledge!
    }
}

public class ShippingCalculator
{
    public decimal Calculate(decimal orderAmount)
    {
        if (orderAmount > 50) return 0; // Same knowledge again!
        return 5.99m;
    }
}

// Refactored - knowledge in one place
public static class ShippingRules
{
    private const decimal FreeShippingThreshold = 50m;
    private const decimal StandardShippingCost = 5.99m;

    public static decimal CalculateShippingCost(decimal itemsTotal)
    {
        return itemsTotal > FreeShippingThreshold ? 0 : StandardShippingCost;
    }
}

// Now all classes use the single source of truth
public class Order
{
    public decimal CalculateTotal()
    {
        var itemsTotal = Items.Sum(item => item.Price);
        return itemsTotal + ShippingRules.CalculateShippingCost(itemsTotal);
    }
}
```

**Decision Framework: Should I Abstract This Duplication?**

Questions to ask before abstracting:

1. **Semantic Check**: Do these code blocks represent the **same business concept** or just happen to look similar?
   - Same concept → Safe to abstract
   - Different concepts → Keep separate

2. **Evolution Check**: If business rules change for one, should the others change too?
   - Yes, always together → Safe to abstract
   - No, independent evolution → Keep separate

3. **Comprehension Check**: Would a developer understand why these are grouped together?
   - Yes, obvious relationship → Safe to abstract
   - No, requires explanation → Keep separate

4. **Coupling Check**: Am I abstracting based on what the code MEANS (semantics) or what it IS (structure)?
   - Based on meaning → Safe to abstract
   - Based on structure → Keep separate

**Example Application:**

```csharp
// CASE 1: Structural similarity, different semantics
public bool ValidatePaymentAmount(decimal amount)
{
    return amount > 0 && amount <= 10000;
}

public bool ValidateTransferAmount(decimal amount)
{
    return amount > 0 && amount <= 10000;
}

// ANALYSIS:
// 1. Semantic: Different concepts (payment limits vs. transfer limits)
// 2. Evolution: Payment limits driven by fraud; transfers by account type
// 3. Comprehension: Not obvious why these would be the same
// 4. Coupling: Based on structure, not meaning
// DECISION: ❌ DO NOT ABSTRACT - Keep separate

// CASE 2: Structural similarity, same semantics
public string FormatUserDisplayName(string first, string last)
{
    return $"{first} {last}".Trim();
}

public string FormatCustomerDisplayName(string first, string last)
{
    return $"{first} {last}".Trim();
}

public string FormatEmployeeDisplayName(string first, string last)
{
    return $"{first} {last}".Trim();
}

// ANALYSIS:
// 1. Semantic: Same concept (how we display person names)
// 2. Evolution: All should change together (e.g., "Last, First" format)
// 3. Comprehension: Obviously the same thing
// 4. Coupling: Based on meaning (all represent "person name display")
// DECISION: ✅ SAFE TO ABSTRACT

public string FormatPersonDisplayName(string first, string last)
{
    return $"{first} {last}".Trim();
}
```

**Remember:** Duplicate code is far cheaper than the wrong abstraction. When in doubt, wait for a third instance to confirm the pattern.

#### 4. Maintain External APIs During Refactoring

Refactoring must never break existing consumers of your code:

```csharp
// Original implementation
public class PaymentProcessor
{
    public ProcessedPayment ProcessPayment(Payment payment)
    {
        // Complex logic all in one function
        if (payment.Amount <= 0)
        {
            throw new InvalidOperationException("Invalid amount");
        }

        if (payment.Amount > 10000)
        {
            throw new InvalidOperationException("Amount too large");
        }

        // ... 50 more lines of validation and processing

        return result;
    }
}

// Refactored - external API unchanged, internals improved
public class PaymentProcessor
{
    public ProcessedPayment ProcessPayment(Payment payment)
    {
        ValidatePaymentAmount(payment.Amount);
        ValidatePaymentMethod(payment.Method);

        var authorizedPayment = AuthorizePayment(payment);
        var capturedPayment = CapturePayment(authorizedPayment);

        return GenerateReceipt(capturedPayment);
    }

    // New internal methods - not public
    private void ValidatePaymentAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Invalid amount");
        }

        if (amount > 10000)
        {
            throw new InvalidOperationException("Amount too large");
        }
    }

    // ...other private methods...
}

// Tests continue to pass without modification because external API unchanged
```

#### 5. Verify and Commit After Refactoring

**CRITICAL**: After every refactoring:

1. Run all tests - they must pass without modification
2. Run static analysis (linting, type checking) - must pass
3. Commit the refactoring separately from feature changes

```bash
# After refactoring
dotnet test          # All tests must pass
dotnet format        # Code formatting must be correct

# Only then commit
git add .
git commit -m "refactor: extract payment validation helpers"
```

### Refactoring Checklist

Before considering refactoring complete, verify:

- [ ] The refactoring actually improves the code (if not, don't refactor)
- [ ] All tests still pass without modification
- [ ] All static analysis tools pass (linting, type checking)
- [ ] No new public APIs were added (only internal ones)
- [ ] Code is more readable than before
- [ ] Any duplication removed was duplication of knowledge, not just code
- [ ] No speculative abstractions were created
- [ ] The refactoring is committed separately from feature changes

### Example Refactoring Session

```csharp
// After getting tests green with minimal implementation:
[TestFixture]
public partial class OrderProcessingSpecs : Specification
{
    [Test]
    public void calculates_total_with_items_and_shipping()
    {
        Given(an_order_with_items_totaling, 55m);
        And(a_shipping_cost_of, 5m);
        When(processing_the_order);
        Then(the_total_should_be, 60m);
    }

    [Test]
    public void applies_free_shipping_over_50()
    {
        Given(an_order_with_items_totaling, 55m);
        And(a_shipping_cost_of, 5m);
        When(processing_the_order);
        Then(the_total_should_be, 55m);
    }
}

// Green implementation (minimal):
public class OrderProcessor
{
    public ProcessedOrder Process(Order order)
    {
        var itemsTotal = order.Items.Sum(item => item.Price);
        var shipping = itemsTotal > 50 ? 0 : order.ShippingCost;
        return new ProcessedOrder
        {
            Items = order.Items,
            Total = itemsTotal + shipping,
            ShippingCost = shipping
        };
    }
}

// Commit the working version
// git commit -m "feat: implement order total calculation with free shipping"

// Assess refactoring opportunities:
// - The variable names could be clearer
// - The free shipping threshold is a magic number
// - The calculation logic could be extracted for clarity
// These improvements would add value, so proceed with refactoring:

public class OrderProcessor
{
    private const decimal FreeShippingThreshold = 50m;

    public ProcessedOrder Process(Order order)
    {
        var itemsTotal = CalculateItemsTotal(order.Items);
        var shipping = CalculateShipping(order.ShippingCost, itemsTotal);
        
        return new ProcessedOrder
        {
            Items = order.Items,
            Total = itemsTotal + shipping,
            ShippingCost = shipping
        };
    }

    private static decimal CalculateItemsTotal(IEnumerable<OrderItem> items)
    {
        return items.Sum(item => item.Price);
    }

    private static decimal CalculateShipping(decimal baseShipping, decimal itemsTotal)
    {
        return itemsTotal > FreeShippingThreshold ? 0 : baseShipping;
    }
}

// Run tests - they still pass!
// Run code formatting - all clean!

// Now commit the refactoring
// git commit -m "refactor: extract order total calculation helpers"
```

### Example: When NOT to Refactor

**Important:** When code doesn't need refactoring, explicitly document this assessment:

```csharp
// After getting this test green:
[TestFixture]
public partial class DiscountCalculationSpecs : Specification
{
    [Test]
    public void applies_10_percent_discount()
    {
        Given(an_original_price_of, 100m);
        And(a_discount_rate_of, 0.1m);
        When(applying_the_discount);
        Then(the_discounted_price_should_be, 90m);
    }
}

// Green implementation:
public class DiscountCalculator
{
    public decimal ApplyDiscount(decimal price, decimal discountRate)
    {
        return price * (1 - discountRate);
    }
}

// REFACTORING ASSESSMENT:
// ✅ Already clean:
//   - Method name clearly expresses intent
//   - Implementation is straightforward calculation
//   - No magic numbers or unclear logic
//   - Pure function with clear inputs/outputs
//
// Conclusion: No refactoring needed. This is fine as-is.

// Commit and move to the next test
// git commit -m "feat: add discount calculation"
```

**Pattern for Refactoring Assessment:**

After every green test, explicitly assess:

1. **Naming**: Are variable/function names clear?
2. **Magic values**: Are constants extracted where needed?
3. **Structure**: Is complexity manageable (nesting ≤2, function <30 lines)?
4. **Duplication**: Is knowledge (not just code) repeated?
5. **Purity**: Are functions pure where possible?

If all are satisfied → **No refactoring needed, commit and move on**

If issues found → Classify priority and refactor if high value

## Commit Guidelines

- Each commit should represent a complete, working change
- Use conventional commits format:
  ```
  feat: add payment validation
  fix: correct date formatting in payment processor
  refactor: extract payment validation logic
  test: add edge cases for payment validation
  ```
- Include test changes with feature changes in the same commit

## Pull Request Standards

- Every PR must have all tests passing
- All linting and quality checks must pass
- Work in small increments that maintain a working state
- PRs should be focused on a single feature or fix
- Include description of the behavior change, not implementation details
