# C# Testing Standards

## Overview

This document outlines the testing standards and practices for projects. These standards ensure consistent, maintainable, and thorough testing aligned with domain-driven design principles.

## BDD Testing

1. **File Organization**: Separate specifications from step implementations using partial classes.
   ```
   FormularyStatusSpecs.cs    // Contains test scenarios and descriptive assertions
   FormularyStatusSteps.cs    // Contains step implementations
   ```

2. **Scenario Structure**: Use the `scenario()` method with Given-When-Then steps.

   ```csharp
   [Test]
   public void formulary_status_must_have_text()
   {
      Given(no_formulary_status);
      When(creating_a_formulary_status);
      Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
   }
   ```

3. **Method Naming**: Use descriptive snake_case method names that express the behavior or state.
   ```csharp
   // In specs:
   Given(no_formulary_status);
   When(creating_a_formulary_status);

   // In step implementations:
   private void no_formulary_status() { /* ... */ }
   private void creating_a_formulary_status() { /* ... */ }
   ```

4. **Multiple Scenarios**: Group related scenarios within a single test method.
   ```csharp
   [Test]
   public void formulary_status_cannot_exceed_100_characters()
   {
       scenario(() =>
       {
           Given(a_formulary_status_of_100_characters);
           When(creating_a_formulary_status);
           Then(it_is_valid);
       });

       scenario(() =>
       {
           Given(a_formulary_status_over_100_characters);
           When(creating_a_formulary_status);
           Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
       });
   }
   ```

5. **Step Implementation**: Implement steps as private methods in the step class.
   ```csharp
   private string formulary_status;

   private void no_formulary_status()
   {
       formulary_status = null;
   }

   private void creating_a_formulary_status()
   {
       validating(() => new FormularyStatus(formulary_status));
   }
   ```

6. **Common Steps Base Class**: Create base classes for common steps across related tests.
   ```csharp
   public partial class AgedCriteriaSpecs : CommonCriteriaSteps<AgeRangeCriterion>
   {
       // Specific tests for aged criteria
   }
   ```

## Validation Testing

1. **Exception Validation**: Use the `validating()` helper method to catch and validate exceptions.
   ```csharp
   private void creating_a_formulary_status()
   {
       validating(() => new FormularyStatus(formulary_status));
   }
   ```

2. **Error Message Validation**: Use the `informs()` method to check for specific error messages.
   ```csharp
   Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
   ```

3. **Multiple Validation Scenarios**: Test both valid and invalid inputs.
   ```csharp
   [Test]
   public void formulary_status_must_have_text()
   {
       scenario(() =>
       {
           Given(no_formulary_status);
           When(creating_a_formulary_status);
           Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
       });

       scenario(() =>
       {
           Given(an_empty_formulary_status);
           When(creating_a_formulary_status);
           Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
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
   FormularyStatusSpecs.cs
   AgedCriteriaSpecs.cs
   ```

3. **Test Attributes**: Use `[TestFixture]` for test classes and `[Test]` for test methods.
   ```csharp
   [TestFixture]
   public partial class FormularyStatusSpecs : Specification
   {
       [Test]
       public void formulary_status_must_have_text()
       {
           // Test scenarios
       }
   }
   ```

4. **Test Methods**: Name test methods to clearly describe the behavior being tested.
   ```csharp
   [Test]
   public void formulary_status_must_have_text()
   
   [Test]
   public void formulary_status_cannot_exceed_100_characters()
   ```

5. **Obsolete Tests**: Mark obsolete tests with the `[Obsolete]` attribute and provide a reason.
   ```csharp
   [Obsolete("Still used by Sphinx")]
   [Test]
   public void legacy_aged_criteria_can_be_created()
   ```

## Test Data Preparation

1. **Test Data Setup**: Use clear, descriptive methods to set up test data.
   ```csharp
   private void a_formulary_status_of_100_characters()
   {
       formulary_status = "".PadRight(100, 'a');
   }

   private void a_formulary_status_over_100_characters()
   {
       formulary_status = "".PadRight(101, 'a');
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
   private string formulary_status;
   ```

## Domain Specific Validation

1. **Domain-Focused Assertions**: Create assertion methods that express domain concepts.
   ```csharp
   private void is_is_for_aged(Aged expected)
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
       Given(a_formulary_status_of_100_characters);  // Exactly at the limit
       When(creating_a_formulary_status);
       Then(it_is_valid);
   });

   scenario(() =>
   {
       Given(a_formulary_status_over_100_characters); // Just over the limit
       When(creating_a_formulary_status);
       Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
   });
   ```

## Multiple Scenario Testing

1. **Complex Domain Rules**: Use multiple scenarios to test different aspects of a domain rule.
   ```csharp
   [Test]
   public void legacy_aged_criteria_can_be_created()
   {
       const int age = 49;

       scenario(() =>
       {
           When(creating_the_legacy_criterion(Criterion.Operators.Equality, age));
           Then(it_has_a_between_operator);
           And(is_is_for_aged(Aged.From(age).To(age)));
           And(() => it_has_a_description_of($"Age is {age} (expressed as years)"));
       });

       scenario(() =>
       {
           When(creating_the_legacy_criterion(Criterion.Operators.GreaterThan, age));
           Then(it_has_a_between_operator);
           And(is_is_for_aged(Aged.From(age + 1)));
           And(() => it_has_a_description_of($"Age is greater than {age} (expressed as years)"));
       });

       // Additional scenarios for other operators
   }
   ```

## Base Classes and Common Functionality

1. **Specification Base Class**: Extend from a common base class for shared functionality.
   ```csharp
   public partial class FormularyStatusSpecs : Specification
   {
       // Tests
   }
   ```

2. **Common Steps**: Create base classes for common step implementations.
   ```csharp
   public partial class AgedCriteriaSpecs : CommonCriteriaSteps<AgeRangeCriterion>
   {
       // Specific tests
   }
   ```

## Test Readability

1. **Fluent Assertions**: Use chained assertions for improved readability.
   ```csharp
   Then(it_has_a_between_operator);
   And(is_is_for_aged(aged));
   And(() => it_has_a_description_of($"Age is between {aged.Minimum} and {aged.Maximum} (expressed as years)"));
   ```

2. **Descriptive Messages**: Use descriptive messages in assertions.
   ```csharp
   Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
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
// FormularyStatusSpecs.cs
[TestFixture]
public partial class FormularyStatusSpecs : Specification
{
    [Test]
    public void formulary_status_validation()
    {
        scenario(() =>
        {
            Given(no_formulary_status);
            When(creating_a_formulary_status);
            Then(() => informs("Formulary status is required and cannot exceed 100 characters."));
        });
        
        scenario(() =>
        {
            Given(a_valid_formulary_status);
            When(creating_a_formulary_status);
            Then(it_is_valid);
        });
    }
}

// FormularyStatusSteps.cs
public partial class FormularyStatusSpecs
{
    private string formulary_status;
    
    private void no_formulary_status()
    {
        formulary_status = null;
    }
    
    private void a_valid_formulary_status()
    {
        formulary_status = "Valid Status";
    }
    
    private void creating_a_formulary_status()
    {
        validating(() => new FormularyStatus(formulary_status));
    }
}
```

### AsyncSpecification Base Class

The `AsyncSpecification` abstract class serves as the foundation for BDD-style tests that need asynchronous operations:

```csharp
public abstract class AsyncSpecification
{
    // BDD step methods
    protected Task Given(Action action) { action.Invoke(); }
    protected Task When(Action action) { action.Invoke(); }
    protected Task Then(Action action) { action.Invoke(); }
    protected Task And(Action action) { action.Invoke(); }
    
    // Multiple scenario support
    protected void scenario(Action test)
    
    // Validation helpers
    protected void validating(Action action)
    protected void informs(string message)
    
    // BDD async step methods
    protected static async Task Given(Func<Task> testAction) { await testAction.Invoke(); }
    protected static async Task And(Func<Task> testAction) { await testAction.Invoke(); }
    protected static async Task When(Func<Task> testAction) { await testAction.Invoke(); }
    protected static async Task Then(Func<Task> testAction) { await testAction.Invoke(); }
    
    // ASync Multiple scenario support
    protected static async Task Scenario(Func<Task> testAction){ await testAction.Invoke(); }

    // Async Validation helpers
    protected static Func<Task> Validating(Func<Task> testAction)
    protected static Func<Task> InformsAsync(string message)
}
```


### Usage in Projects

When writing tests for projects, the recommended approach is:

1. Create a partial class that inherits from `AsyncSpecification`
2. Split specifications and steps into separate files
3. Use the fluent assertions for validations

Example using the framework:

```csharp
// AsyncExampleShould.cs
[TestFixture]
public partial class AsyncExampleShould : AsyncSpecification
{
    [Test]
    public async Task pass_our_first_behavioural_test_async()
    {
        await Given(two_numbers_from_a_remote_source);
              When(we_give_them_to_our_complex_system);
        await Then(we_get_the_sum_from_a_remote_source);
              And(we_can_validate_something_else);
    }
}

// AsyncExampleSteps.cs
public partial class AsyncExampleShould
{
    private int sum;
    private ComplexSystem complex_system;
    private int first_number;
    private int second_number;

    protected override Task before_each()
    {
        base.before_each();
        sum = 0;
        first_number = 0;
        second_number = 0;
        complex_system = new ComplexSystem(new MagicDependency());
        return Task.CompletedTask;
    }

    private Task two_numbers_from_a_remote_source()
    {
        first_number = 0;
        second_number = 2;
        return Task.CompletedTask;
    }

    private void we_give_them_to_our_complex_system()
    {
        sum = complex_system.Sum(first_number, second_number);
    }

    private Task we_get_the_sum_from_a_remote_source()
    {
        sum = first_number + second_number;
        return Task.CompletedTask;
    }
    
    private static void we_can_validate_something_else(){}
}
```

## Conclusion

These testing standards aim to ensure that the codebase maintains a consistent approach to testing that aligns with domain-driven design principles. The BDD-style testing approach with clear separation of specifications and step implementations improves readability and helps focus tests on domain behaviors rather than implementation details. 

The `Testing.Bdd` package provides utilities that support these standards and make it easier to write clean, expressive tests.