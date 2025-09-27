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
    protected void it_is_valid()
    
    // Helper methods
    protected string text_of_length(uint chars)
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

## Database Integration Testing

When testing code that interacts with databases, the `DbSpecification` class provides automatic transaction management that eliminates the need for manual test data cleanup.

### DbSpecification Base Class

The `DbSpecification` class extends the standard `Specification` class and adds automatic transaction management:

```csharp
public abstract class DbSpecification : Specification
{
    private TransactionScope? scope;

    protected override void before_each()
    {
        base.before_each();
        scope?.Dispose();

        var transactionOptions = new TransactionOptions 
        { 
            IsolationLevel = IsolationLevel.ReadCommitted, 
            Timeout = TimeSpan.FromMinutes(2) 
        };
        scope = new TransactionScope(TransactionScopeOption.RequiresNew, 
            transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
    }

    protected override void after_each()
    {
        base.after_each();
        scope?.Dispose();
    }
}
```

### Key Features

1. **Automatic Transaction Management**: Each test runs within its own transaction scope
2. **Automatic Rollback**: All database changes are automatically rolled back after each test
3. **Test Isolation**: Tests don't interfere with each other's data
4. **Async Support**: Transactions flow properly through async operations
5. **No Manual Cleanup**: Eliminates the need for complex setup/teardown logic

### Usage Examples

#### Basic Database Integration Test

```csharp
[TestFixture]
public partial class CustomerRepositorySpecs : DbSpecification
{
    [Test]
    public void creating_and_retrieving_customer()
    {
        scenario(() =>
        {
            Given(a_new_customer);
            When(saving_the_customer);
            Then(the_customer_can_be_retrieved);
        });
    }
}

public partial class CustomerRepositorySpecs
{
    private Customer _customer;
    private ICustomerRepository _repository;
    private Guid _savedCustomerId;

    private void a_new_customer()
    {
        _customer = new Customer("John Doe", "john@example.com");
        _repository = new CustomerRepository(connectionFactory);
    }

    private async Task saving_the_customer()
    {
        await _repository.AddAsync(_customer);
        _savedCustomerId = _customer.Id;
    }

    private async Task the_customer_can_be_retrieved()
    {
        var retrieved = await _repository.GetByIdAsync(_savedCustomerId);
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be("John Doe");
    }
}
```

#### Repository Integration Testing

```csharp
[TestFixture]
public partial class OrderRepositorySpecs : DbSpecification
{
    [Test]
    public void order_operations_maintain_data_integrity()
    {
        scenario(() =>
        {
            Given(an_order_with_items);
            When(saving_the_complete_order);
            Then(all_order_data_is_persisted_correctly);
        });

        scenario(() =>
        {
            Given(an_existing_order);
            When(adding_items_to_the_order);
            Then(the_order_total_is_updated_correctly);
        });
    }
}

public partial class OrderRepositorySpecs
{
    private Order _order;
    private IOrderRepository _orderRepository;
    private readonly IDbConnectionFactory _connectionFactory = Services.TestDatabase;

    private void an_order_with_items()
    {
        _order = new Order();
        _order.AddItem(new Product("Product A", 10.00m), 2);
        _order.AddItem(new Product("Product B", 15.00m), 1);
        _orderRepository = new OrderRepository(_connectionFactory);
    }

    private async Task saving_the_complete_order()
    {
        await _orderRepository.AddAsync(_order);
    }

    private async Task all_order_data_is_persisted_correctly()
    {
        var retrieved = await _orderRepository.GetByIdAsync(_order.Id);
        retrieved.Should().NotBeNull();
        retrieved.Items.Should().HaveCount(2);
        retrieved.TotalAmount.Should().Be(35.00m);
    }
}
```

### Best Practices

#### When to Use DbSpecification

- **Use `DbSpecification`** for tests that require actual database interactions:
  - Repository integration tests
  - Data access layer testing  
  - End-to-end scenarios involving database operations
  - Testing complex queries and stored procedures

- **Use `Specification`** for unit tests that don't require database access:
  - Domain model validation
  - Business logic testing
  - Service layer tests with mocked repositories

#### Database Test Guidelines

1. **Test Real Database Behavior**: Use actual database connections, not in-memory substitutes
2. **Focus on Integration Points**: Test how your code interacts with the database schema
3. **Verify Data Integrity**: Ensure transactions, constraints, and relationships work correctly
4. **Test Complex Scenarios**: Multi-table operations, cascading deletes, concurrent access

#### Performance Considerations

```csharp
[TestFixture]
public partial class PerformanceCriticalSpecs : DbSpecification
{
    [Test]
    public void bulk_operations_complete_within_acceptable_time()
    {
        scenario(() =>
        {
            Given(a_large_dataset);
            When(performing_bulk_insert);
            Then(operation_completes_within_time_limit);
        });
    }

    private void operation_completes_within_time_limit()
    {
        // The transaction timeout (2 minutes) ensures tests don't run indefinitely
        // Consider the automatic rollback cost for very large datasets
    }
}
```

### Integration with Repository Patterns

The `DbSpecification` class works seamlessly with repository patterns and the CQRS approach:

```csharp
[TestFixture] 
public partial class ProductCatalogSpecs : DbSpecification
{
    [Test]
    public void product_lifecycle_operations()
    {
        scenario(() =>
        {
            Given(a_product_catalog_service);
            When(adding_a_new_product);
            Then(the_product_appears_in_search_results);
            And(product_audit_trail_is_created);
        });
    }
}

public partial class ProductCatalogSpecs
{
    private ProductCatalogService _service;
    private Product _newProduct;

    private void a_product_catalog_service()
    {
        var writeRepository = new ProductRepository(_connectionFactory);
        var readRepository = new ProductQueryRepository(_connectionFactory);
        var eventPublisher = new DomainEventPublisher();
        
        _service = new ProductCatalogService(writeRepository, readRepository, eventPublisher);
        _newProduct = new Product("Test Product", "Description", 99.99m);
    }

    private async Task adding_a_new_product()
    {
        await _service.AddProductAsync(_newProduct);
    }

    private async Task the_product_appears_in_search_results()
    {
        var results = await _service.SearchProductsAsync("Test Product");
        results.Should().Contain(p => p.Id == _newProduct.Id);
    }

    private async Task product_audit_trail_is_created()
    {
        var auditEntries = await _service.GetAuditTrailAsync(_newProduct.Id);
        auditEntries.Should().Contain(e => e.Action == "ProductAdded");
    }
}
```

### Transaction Scope Configuration

The `DbSpecification` class uses these transaction settings:

- **Isolation Level**: `ReadCommitted` - Prevents dirty reads while allowing concurrent access
- **Timeout**: 2 minutes - Sufficient for most test scenarios while preventing runaway tests  
- **Scope Option**: `RequiresNew` - Each test gets its own transaction
- **Async Flow**: `Enabled` - Transactions flow correctly through async/await operations

These settings balance test isolation, performance, and reliability for database integration testing.

## Conclusion

These testing standards aim to ensure that the codebase maintains a consistent approach to testing that aligns with domain-driven design principles. The BDD-style testing approach with clear separation of specifications and step implementations improves readability and helps focus tests on domain behaviors rather than implementation details. 

The `Testing.Bdd` package provides utilities that support these standards and make it easier to write clean, expressive tests. For database integration scenarios, the `DbSpecification` class eliminates the complexity of test data management through automatic transaction rollback, enabling reliable and isolated database testing.
