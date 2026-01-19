# C# Testing Standards

## Overview

This document outlines the testing standards and practices for projects. These standards ensure consistent, maintainable, and thorough testing aligned with domain-driven design principles.

## BDD Testing

1. **File Organization**: Separate specifications from step implementations using partial classes.
   ```
   AccountTypeSpecs.cs    // Contains test scenarios and descriptive assertions
   AccountTypeSteps.cs    // Contains step implementations
   ```

2. **Scenario Structure**: Use the `scenario()` method with Given-When-Then steps.

   ```csharp
   [Test]
   public void account_type_must_have_text()
   {
      Given(no_account_type);
      When(creating_a_account_type);
      Then(() => informs("Account type is required and cannot exceed 100 characters."));
   }
   ```

3. **Method Naming**: Use descriptive snake_case method names that express the behavior or state.
   ```csharp
   // In specs:
   Given(no_account_type);
   When(creating_a_account_type);

   // In step implementations:
   private void no_account_type() { /* ... */ }
   private void creating_a_account_type() { /* ... */ }
   ```

4. **Multiple Scenarios**: Group related scenarios within a single test method.
   ```csharp
   [Test]
   public void account_type_cannot_exceed_100_characters()
   {
       scenario(() =>
       {
           Given(a_account_type_of_100_characters);
           When(creating_a_account_type);
           Then(it_is_valid);
       });

       scenario(() =>
       {
           Given(a_account_type_over_100_characters);
           When(creating_a_account_type);
           Then(() => informs("Account type is required and cannot exceed 100 characters."));
       });
   }
   ```

5. **Step Implementation**: Implement steps as private methods in the step class.
   ```csharp
   private string account_type;

   private void no_account_type()
   {
       account_type = null;
   }

   private void creating_a_account_type()
   {
       validating(() => new AccountType(account_type));
   }
   ```

6. **Common Steps Base Class**: Create base classes for common steps across related tests.
   ```csharp
   public partial class PriceTierCriteriaSpecs : CommonCriteriaSteps<PriceTierCriterion>
   {
       // Specific tests for aged criteria
   }
   ```

## Validation Testing

1. **Exception Validation**: Use the `validating()` helper method to catch and validate exceptions.
   ```csharp
   private void creating_a_account_type()
   {
       validating(() => new AccountType(account_type));
   }
   ```

2. **Error Message Validation**: Use the `informs()` method to check for specific error messages.
   ```csharp
   Then(() => informs("Account type is required and cannot exceed 100 characters."));
   ```

3. **Multiple Validation Scenarios**: Test both valid and invalid inputs.
   ```csharp
   [Test]
   public void account_type_must_have_text()
   {
       scenario(() =>
       {
           Given(no_account_type);
           When(creating_a_account_type);
           Then(() => informs("Account type is required and cannot exceed 100 characters."));
       });

       scenario(() =>
       {
           Given(an_empty_account_type);
           When(creating_a_account_type);
           Then(() => informs("Account type is required and cannot exceed 100 characters."));
       });
   }
   ```

## Test Organization

1. **Namespaces**: Organize tests to match the production code structure.
   ```csharp
   namespace Testing.Primitives;
   namespace Testing.Primitives.Execution.Criteria;
   ```

2. **Class and File Naming**: Use descriptive names with a "Specs" suffix.
   ```csharp
   AccountTypeSpecs.cs
   PriceTierCriteriaSpecs.cs
   ```

3. **Test Attributes**: Use `[TestFixture]` for test classes and `[Test]` for test methods.
   ```csharp
   [TestFixture]
   public partial class AccountTypeSpecs : Specification
   {
       [Test]
       public void account_type_must_have_text()
       {
           // Test scenarios
       }
   }
   ```

4. **Test Methods**: Name test methods to clearly describe the behavior being tested.
   ```csharp
   [Test]
   public void account_type_must_have_text()
   
   [Test]
   public void account_type_cannot_exceed_100_characters()
   ```

5. **Obsolete Tests**: Mark obsolete tests with the `[Obsolete]` attribute and provide a reason.
   ```csharp
   [Obsolete("Still used by legacy system")]
   [Test]
   public void legacy_price_tier_criteria_can_be_created()
   ```

## Test Data Preparation

1. **Test Data Setup**: Use clear, descriptive methods to set up test data.
   ```csharp
   private void a_account_type_of_100_characters()
   {
       account_type = "".PadRight(100, 'a');
   }

   private void a_account_type_over_100_characters()
   {
       account_type = "".PadRight(101, 'a');
   }
   ```

2. **Common Test Data**: Extract common test data to reusable methods.
   ```csharp
   // In a base class or shared helper
   protected Product CreateDefaultProduct() 
   {
       // Implementation
   }
   ```

3. **Test State**: Store test state in private fields in the step implementation class.
   ```csharp
   private string account_type;
   ```

## Domain Specific Validation

1. **Domain-Focused Assertions**: Create assertion methods that express domain concepts.
   ```csharp
   private void is_for_price_tier(PriceTier expected)
   {
       // Implementation that checks if the criterion is for the expected age
   }
   
   private void it_has_a_description_of(string expected)
   {
       // Implementation that checks for the expected description
   }
   ```

2. **Boundary Testing**: Test boundary conditions for domain validation rules.
   ```csharp
   scenario(() =>
   {
       Given(a_account_type_of_100_characters);  // Exactly at the limit
       When(creating_a_account_type);
       Then(it_is_valid);
   });

   scenario(() =>
   {
       Given(a_account_type_over_100_characters); // Just over the limit
       When(creating_a_account_type);
       Then(() => informs("Account type is required and cannot exceed 100 characters."));
   });
   ```

## Multiple Scenario Testing

1. **Complex Domain Rules**: Use multiple scenarios to test different aspects of a domain rule.
   ```csharp
   [Test]
   public void legacy_price_tier_criteria_can_be_created()
   {
       const decimal amount = 49.99m;

       scenario(() =>
       {
           When(creating_the_legacy_criterion(Criterion.Operators.Equality, amount));
           Then(it_has_a_between_operator);
           And(is_for_price_tier(PriceTier.From(amount).To(amount)));
           And(() => it_has_a_description_of($"Price is {amount} (expressed as currency)"));
       });

       scenario(() =>
       {
           When(creating_the_legacy_criterion(Criterion.Operators.GreaterThan, amount));
           Then(it_has_a_between_operator);
           And(is_for_price_tier(PriceTier.From(amount + 1)));
           And(() => it_has_a_description_of($"Price is greater than {amount} (expressed as currency)"));
       });

       // Additional scenarios for other operators
   }
   ```

## Base Classes and Common Functionality

1. **Specification Base Class**: Extend from a common base class for shared functionality.
   ```csharp
   public partial class AccountTypeSpecs : Specification
   {
       // Tests
   }
   ```

2. **Common Steps**: Create base classes for common step implementations.
   ```csharp
   public partial class PriceTierCriteriaSpecs : CommonCriteriaSteps<PriceTierCriterion>
   {
       // Specific tests
   }
   ```

## Test Readability

1. **Fluent Assertions**: Use chained assertions for improved readability.
   ```csharp
   Then(it_has_a_between_operator);
   And(is_for_price_tier(priceTier));
   And(() => it_has_a_description_of($"Price is between {priceTier.Minimum} and {priceTier.Maximum} (expressed as currency)"));
   ```

2. **Descriptive Messages**: Use descriptive messages in assertions.
   ```csharp
   Then(() => informs("Account type is required and cannot exceed 100 characters."));
   ```

## Test Framework

The TestingLibrary repository provides the `Testing.bdd` package, which includes BDD testing utilities that align with our testing standards. This section outlines the key components of this framework.

### Specification Base Class

The `Specification` abstract class serves as the foundation for BDD-style tests:

```csharp
public abstract class Specification
{
    // BDD step methods
    protected void Given(Action action) { action.Invoke(); }
    protected void When(Action action) { action.Invoke(); }
    protected void Then(Action action) { action.Invoke(); }
    protected void And(Action action) { action.Invoke(); }
    
    // Multiple scenario support
    protected void scenario(Action test)
    
    // Validation helpers
    protected void validating(Action action)
    protected void informs(string message)
}
```

### Usage in Projects

When writing tests for projects, the recommended approach is:

1. Create a partial class that inherits from `Specification`
2. Split specifications and steps into separate files
3. Use the fluent assertions for validations

Example using the framework:

```csharp
// AccountTypeSpecs.cs
[TestFixture]
public partial class AccountTypeSpecs : Specification
{
    [Test]
    public void account_type_validation()
    {
        scenario(() =>
        {
            Given(no_account_type);
            When(creating_a_account_type);
            Then(() => informs("Account type is required and cannot exceed 100 characters."));
        });
        
        scenario(() =>
        {
            Given(a_valid_account_type);
            When(creating_a_account_type);
            Then(it_is_valid);
        });
    }
}

// AccountTypeSteps.cs
public partial class AccountTypeSpecs
{
    private string account_type;
    
    private void no_account_type()
    {
        account_type = null;
    }
    
    private void a_valid_account_type()
    {
        account_type = "Valid Status";
    }
    
    private void creating_a_account_type()
    {
        validating(() => new AccountType(account_type));
    }
}
```

### AsyncSpecification Base Class

The `AsyncSpecification` abstract class serves as the foundation for BDD-style tests that need asynchronous operations. It provides **both synchronous and asynchronous** step methods - use synchronous methods when async is not needed.

```csharp
public abstract class AsyncSpecification
{
    // Synchronous BDD step methods (use when async not needed)
    protected void Given(Action action) { action.Invoke(); }
    protected void When(Action action) { action.Invoke(); }
    protected void Then(Action action) { action.Invoke(); }
    protected void And(Action action) { action.Invoke(); }

    // Async BDD step methods (use only when async is required)
    protected static async Task Given(Func<Task> testAction) { await testAction.Invoke(); }
    protected static async Task And(Func<Task> testAction) { await testAction.Invoke(); }
    protected static async Task When(Func<Task> testAction) { await testAction.Invoke(); }
    protected static async Task Then(Func<Task> testAction) { await testAction.Invoke(); }

    // Multiple scenario support
    protected void scenario(Action test)
    protected static async Task Scenario(Func<Task> testAction){ await testAction.Invoke(); }

    // Validation helpers
    protected void validating(Action action)
    protected void informs(string message)
    protected static Func<Task> Validating(Func<Task> testAction)
    protected static Func<Task> InformsAsync(string message)
}
```

**Key principle**: Do not use `async`/`await` for methods that do not need it. Use regular `void` methods for synchronous steps.

### Usage in Projects

When writing tests for projects, the recommended approach is:

1. Create a partial class that inherits from `AsyncSpecification`
2. Split specifications and steps into separate files
3. Use synchronous (`void`) methods for steps that don't need async
4. Only use `async Task` for steps that perform actual async operations (HTTP calls, database queries, etc.)
5. Only use `await` in the spec for steps that return `Task`

Example using the framework (from EventApiSpecs):

```csharp
// EventApi.specs.cs
[TestFixture]
public partial class EventApiSpecs : AsyncSpecification
{
    [Test]
    public async Task can_create_event()
    {
              Given(an_admin_user_exists);           // sync - no await
              And(a_request_to_create_an_event);    // sync - no await
        await When(creating_the_event);              // async - needs await
        await And(requesting_the_event);             // async - needs await
        await Then(the_event_is_created);            // async - needs await
              And(an_integration_event_is_published); // sync - no await
    }

    [Test]
    public async Task a_non_admin_cannot_create_event()
    {
              Given(an_admin_user_exists);
              And(a_request_to_create_an_event_as_a_non_admin_user);
        await When(creating_the_event_that_should_fail);
              Then(the_event_creation_is_forbidden); // sync assertion - no await
    }
}

// EventApi.steps.cs
public partial class EventApiSpecs
{
    private HttpClient client = null!;
    private HttpContent content = null!;
    private HttpStatusCode response_code;

    // Synchronous step - regular void method
    private void an_admin_user_exists() {}

    // Synchronous step - sets up test data, no async needed
    private void a_request_to_create_an_event()
    {
        content = new StringContent(
            JsonSerialization.Serialize(new EventPayload(...)),
            Encoding.UTF8,
            "application/json");
    }

    // Async step - performs HTTP call
    private async Task creating_the_event()
    {
        var response = await client.PostAsync(Routes.Events, content);
        response_code = response.StatusCode;
        // ...
    }

    // Async step - reads response content
    private async Task the_event_is_created()
    {
        var theEvent = JsonSerialization.Deserialize<Event>(await content.ReadAsStringAsync());
        response_code.ShouldBe(HttpStatusCode.OK);
        // ...
    }

    // Synchronous step - simple assertion, no async needed
    private void the_event_creation_is_forbidden()
    {
        response_code.ShouldBe(HttpStatusCode.Forbidden);
    }

    // Synchronous step - checks published messages (sync API)
    private void an_integration_event_is_published()
    {
        testHarness.Published.Select<EventUpserted>()
            .Any(e => e.Context.Message.Id == returned_id)
            .ShouldBeTrue("Event was not published to the bus");
    }
}
```

## Conclusion

These testing standards aim to ensure that the codebase maintains a consistent approach to testing that aligns with domain-driven design principles. The BDD-style testing approach with clear separation of specifications and step implementations improves readability and helps focus tests on domain behaviors rather than implementation details. 

The `Testing.Bdd` package provides utilities that support these standards and make it easier to write clean, expressive tests.