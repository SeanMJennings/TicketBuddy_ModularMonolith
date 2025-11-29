# Code Quality Standards

This document provides guidelines for creating high-quality, maintainable code in projects.

## Core Principles of Simplicity

These principles are foundational to avoiding unnecessary complexity.

- **Incidental vs. Essential Complexity:** Strive to minimize *incidental complexity* (self-imposed, arising from tools and choices) and focus only on the *essential complexity* (inherent to the problem). For example, choosing a complex framework for a simple task adds incidental complexity.
- **KISS (Keep It Simple, Stupid):** Prioritize simplicity. If a problem can be solved with a simple function, don't build a multi-layered class hierarchy.
- **YAGNI (You Ain't Gonna Need It):** Avoid adding functionality until it is absolutely necessary. Don't write a generic "export to CSV/XML/JSON" feature when only CSV is required.
- **DRY (Don't Repeat Yourself):** Abstract common functionality. If you find yourself copying and pasting code, it's a sign that you need to create a reusable function or component.

## SOLID Principles for Object-Oriented Design

These five principles are the bedrock of building maintainable and scalable object-oriented systems.

- **Single Responsibility Principle (SRP):** A class should have only one reason to change.
  - *Example:* Instead of a `User` class that handles both user data and database operations, create a `User` class for data and a `UserRepository` class for persistence.
- **Open/Closed Principle (OCP):** Software entities should be open for extension but closed for modification.
  - *Example:* Use interfaces and dependency injection. To add a new payment method, create a new class that implements `IPaymentMethod` instead of modifying an existing `PaymentService` class with a large `switch` statement.
- **Liskov Substitution Principle (LSP):** Subtypes must be substitutable for their base types.
  - *Example:* If you have a `Bird` class with a `fly()` method, a `Penguin` subclass would violate LSP because penguins can't fly. This suggests a different abstraction is needed, perhaps separating flying birds from non-flying birds.
- **Interface Segregation Principle (ISP):** Clients should not be forced to depend on interfaces they do not use.
  - *Example:* Instead of a single `IWorker` interface with `work()` and `eat()` methods, create separate `IWorkable` and `IEatable` interfaces. A robot worker would only implement `IWorkable`.
- **Dependency Inversion Principle (DIP):** High-level modules should depend on abstractions, not on low-level modules.
  - *Example:* A `NotificationService` should depend on an `IMessageSender` interface, not a concrete `EmailSender` or `SMSSender` class. This allows you to easily swap out the sending mechanism without changing the service.

## Recognizing and Avoiding Anti-Patterns

Be vigilant for common anti-patterns that degrade code quality.

- **God Object:** A massive class that does everything.
  - *Symptom:* A class with hundreds of methods and thousands of lines of code.
  - *Solution:* Break it down into smaller, cohesive classes, each with a single responsibility.
- **Spaghetti Code:** Code with a tangled, hard-to-follow control structure.
  - *Symptom:* Deeply nested loops and conditionals, excessive `goto` statements.
  - *Solution:* Refactor to create clear, linear execution paths using smaller functions and well-defined control flows.
- **Premature Optimization:** Optimizing code before performance metrics show it is required.
  - *Symptom:* Complex, hard-to-read code written for a minor or non-existent performance gain.
  - *Solution:* Write clean, simple code first. Only optimize when profiling reveals a bottleneck.
- **Magic Numbers/Strings:** Unexplained values in code.
  - *Symptom:* `if (user.Status == 2)`
  - *Solution:* Replace with named constants: `if (user.Status == UserStatus.Active)`

### Why Self-Documenting Code Matters

- **Safety**: Clear, unambiguous code reduces misinterpretation that could affect patient care
- **Maintainability**: Future developers can understand and modify code without extensive documentation
- **Domain Alignment**: Code that mirrors business language improves communication with domain experts
- **Reduced Cognitive Load**: Developers spend less time deciphering what code does and more time on business logic
- **Living Documentation**: Code serves as up-to-date documentation that never goes stale

### Integration with Standards

Self-documenting code standards complement and reinforce other development practices:
- **Domain-Driven Design**: Code reflects the ubiquitous language of the business domain
- **BDD Testing**: Tests serve as living specifications that document expected behavior
- **Code Quality**: Clean, expressive code is inherently higher quality and more maintainable

### Core Principles

#### Intent-Revealing Code Over Comments

**Principle**: The code itself should reveal what it does and why it does it. Comments should explain context, not mechanics.

```csharp
// AVOID: Comments explaining what the code does
// Check if the user is over 18
if (user.Age >= 18)

// PREFER: Self-revealing code
if (user.IsAdult())

// AVOID: Magic numbers requiring explanation
if (order.Status == 3) // 3 means "shipped"

// PREFER: Expressive enums
if (order.Status == OrderStatus.Shipped)
```

#### Domain Language Consistency

**Principle**: Use the same terminology in code that domain experts use in conversation.

```csharp
// AVOID: Technical terminology
public class DataProcessor
{
    public void ProcessRecords(List<Record> records) { }
}

// PREFER: Domain language
public class PrescriptionValidator
{
    public void ValidatePrescriptions(List<Prescription> prescriptions) { }
}
```

#### Fail-Fast with Clear Messages

**Principle**: When something goes wrong, make it immediately obvious what happened and why.

```csharp
// AVOID: Generic error messages
throw new Exception("Invalid input");

// PREFER: Specific, actionable error messages
throw new InvalidPrescriptionException(
    $"Prescription {prescriptionId} cannot be dispensed: patient allergy to {allergen} detected");
```

#### Code Readability Hierarchy

**Principle**: Code should be readable at multiple levels - class, method, and line level.

1. **Class Level**: Clear responsibility and purpose
2. **Method Level**: Single responsibility with descriptive name
3. **Line Level**: Each statement's purpose is obvious

### Naming Standards

#### Class Naming

**Classes should clearly indicate their role and responsibility:**

```csharp
// GOOD: Clear responsibility
public class DrugInteractionChecker { }
public class PatientEligibilityValidator { }

// AVOID: Vague or technical names
public class DataHelper { }
public class Manager { }
public class Processor { }
```

**Naming Patterns:**
- **Services**: Use domain verbs (`DrugInteractionChecker`, `PrescriptionValidator`)
- **Entities**: Use domain nouns (`Patient`, `Prescription`, `Formulary`)
- **Primitives**: Use descriptive domain concepts (`Money`, `PatientId`, `DrugCode`)
- **Repositories**: Use `Repository` suffix (`PatientRepository`, `PrescriptionRepository`)

#### Method Naming

**Methods should clearly express what they do and return:**

```csharp
// GOOD: Action-oriented, specific
public bool HasAllergiesTo(Drug drug)
public Money CalculateCopay(Insurance insurance, Drug drug)
public void DispensePrescription(Prescription prescription)

// AVOID: Vague or abbreviated
public bool Check(object obj)
public decimal Calc(object x, object y)
public void Process(object data)
public object Validate()
```

**Naming Guidelines:**
- **Boolean methods**: Start with `Is`, `Has`, `Can`, `Should`
- **Action methods**: Use clear verbs (`Calculate`, `Validate`, `Dispense`)
- **Query methods**: Describe what is returned (`GetEligiblePatients`, `FindActivePrescriptions`)
- **Avoid abbreviations**: `CalculateCopayment` not `CalcCopay`

#### Variable Naming

**Variables should clearly indicate their purpose and content:**

```csharp
// GOOD: Self-explanatory
var eligiblePatients = patientService.GetEligiblePatients();
var maximumDosage = drug.GetMaximumDailyDosage();
var insuranceApprovalRequired = prescription.RequiresPreAuthorization();

// AVOID: Cryptic or generic
var list = service.Get();
var max = drug.GetMax();
var flag = prescription.Check();
```

#### Constants and Configuration

**Use descriptive names that explain business meaning:**

```csharp
// GOOD: Business meaning clear
public static class BusinessRules
{
    public const int MaximumPrescriptionDurationDays = 90;
    public const decimal MinimumInsuranceCopayAmount = 5.00m;
    public const int PatientEligibilityGracePeriodDays = 30;
}

// AVOID: Technical or unclear names
public const int MAX_DAYS = 90;
public const decimal MIN_AMT = 5.00m;
public const int GRACE = 30;
```

### Method Design

#### Single Responsibility Methods

**Each method should do one thing and do it well:**

```csharp
// GOOD: Single, clear responsibility
public ValidationResult ValidatePatientEligibility(Patient patient, Insurance insurance)
{
    if (!patient.IsActive())
        return ValidationResult.Failed("Patient is not active");

    if (!insurance.CoversPatient(patient))
        return ValidationResult.Failed("Insurance does not cover this patient");

    return ValidationResult.Success();
}

// AVOID: Multiple responsibilities
public bool ProcessPatient(Patient patient, Insurance insurance, List<Prescription> prescriptions)
{
    // Validates patient, processes insurance, dispenses prescriptions
    // Too many responsibilities in one method
}
```

#### Clear Parameter Design

**Method parameters should be specific and meaningful:**

```csharp
// GOOD: Specific, typed parameters
public void TransferPrescription(
    PrescriptionId prescriptionId, 
    PharmacyId fromPharmacy, 
    PharmacyId toPharmacy,
    TransferReason reason)

// AVOID: Generic or primitive parameters
public void Transfer(string id, string from, string to, int reasonCode)
```

#### Expressive Return Types

**Return types should clearly indicate what the method provides:**

```csharp
// GOOD: Specific return types
public EligibilityResult CheckPatientEligibility(PatientId patientId)
public Maybe<Prescription> FindActivePrescription(PatientId patientId, DrugId drugId)
public ValidationResult ValidateInsuranceCoverage(Insurance insurance)

// AVOID: Generic return types
public bool Check(string id)
public object Find(string patientId, string drugId)
public string Validate(object insurance)
```

#### Method Length Guidelines

**Methods should be concise and focused:**

- **Ideal**: 5-15 lines for most methods
- **Maximum**: 25 lines (consider refactoring beyond this)
- **Exception**: Complex domain logic may require longer methods if they remain readable

```csharp
// GOOD: Concise and focused
public Money CalculateInsuranceCopay(Prescription prescription, Insurance insurance)
{
    var baseCost = prescription.Drug.Cost;
    var coveragePercentage = insurance.GetCoveragePercentage(prescription.Drug);
    var copayAmount = baseCost * (1 - coveragePercentage);

    return Money.FromDecimal(copayAmount, Currency.USD);
}

// Consider refactoring when methods become too long or have multiple concerns
```

### Primitive Obsession

Primitive obsession occurs when we use built-in language types (string, int, decimal) to represent domain concepts instead of creating specific types. This practice severely hampers code self-documentation because it removes business meaning from the code.

#### The Problem with Primitives

**Primitive obsession masks domain concepts:**

```csharp
// PROBLEMATIC: What do these strings and decimals represent?
public void ProcessPayment(string patientId, string pharmacyId, decimal amount, string currency)
{
    // Is patientId a GUID? Social Security Number? Internal ID?
    // What format should pharmacyId be in?
    // What currency codes are valid?
    // Can amount be negative?
}

// Method call provides no clarity
ProcessPayment("12345", "PH789", 25.50m, "USD");
```

#### Domain Primitives for Clarity

**Create specific types that express business meaning:**

```csharp
// BETTER: Domain concepts are explicit
public void ProcessPayment(PatientId patientId, PharmacyId pharmacyId, Money amount)
{
    // Clear what each parameter represents
    // Types enforce valid construction
    // Business rules are embedded in the types
}

// Method call is self-documenting
ProcessPayment(
    PatientId.FromGuid(patientGuid),
    PharmacyId.FromString("PH789"),
    Money.Dollars(25.50m));
```

#### Primitive Implementation Examples

**Money Primitive:**

```csharp
public readonly struct Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new ArgumentException("Money amount cannot be negative", nameof(amount));
        Amount = amount;
        Currency = currency;
    }

    public static Money Dollars(decimal amount) => new(amount, Currency.USD);
    public static Money Zero(Currency currency) => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} to {other.Currency}");
        return new Money(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:C} {Currency}";

    public bool Equals(Money other) => Amount == other.Amount && Currency == other.Currency;
    public override bool Equals(object obj) => obj is Money other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}
```

#### Descriptive Enums Over Magic Values

**Use enums to make states and options explicit:**

```csharp
// AVOID: Magic numbers and strings
public class Prescription
{
    public int Status { get; set; } // What does 1, 2, 3 mean?
}

// PREFER: Self-documenting enums
public class Prescription
{
    public PrescriptionStatus Status { get; set; }
}

public enum PrescriptionStatus
{
    Pending,
    Approved,
    Dispensed,
    Cancelled,
    Expired
}
```

#### Type Safety Patterns

**Design types to make illegal states unrepresentable:**

```csharp
// PROBLEMATIC: Nothing prevents invalid combinations
public class Prescription
{
    public string Status { get; set; }
    public DateTime? DispensedAt { get; set; }
    public string PharmacistId { get; set; }
}

// Can create: Status = "Pending", DispensedAt = DateTime.Now (invalid!)

// BETTER: Types prevent invalid states
public abstract class Prescription
{
    public PrescriptionId Id { get; }
    public PatientId PatientId { get; }
    public DrugId DrugId { get; }

    protected Prescription(PrescriptionId id, PatientId patientId, DrugId drugId)
    {
        Id = id;
        PatientId = patientId;
        DrugId = drugId;
    }
}

public class PendingPrescription : Prescription
{
    // Pending prescriptions cannot have dispense information
    public DateTime CreatedAt { get; }
}

public class DispensedPrescription : Prescription  
{
    // Dispensed prescriptions must have this information
    public DateTime DispensedAt { get; }
    public PharmacistId DispensedBy { get; }

    public DispensedPrescription(
        PrescriptionId id,
        PatientId patientId,
        DrugId drugId,
        DateTime dispensedAt,
        PharmacistId dispensedBy) : base(id, patientId, drugId)
    {
        DispensedAt = dispensedAt;
        DispensedBy = dispensedBy;
    }
}
```

#### Integration with Domain Models

**Primitives support rich domain models:**

```csharp
public class Patient
{
    public PatientId Id { get; }
    public PatientName Name { get; }
    public DateOfBirth DateOfBirth { get; }
    public Insurance Insurance { get; }
    private readonly List<Allergy> _allergies = new();

    public Patient(PatientId id, PatientName name, DateOfBirth dateOfBirth)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DateOfBirth = dateOfBirth;
    }

    public bool IsAllergicTo(Drug drug)
    {
        return _allergies.Any(allergy => allergy.ConflictsWith(drug));
    }

    public Age CalculateAge(Date asOfDate)
    {
        return Age.Between(DateOfBirth.Value, asOfDate);
    }
}
```

### Code Structure & Organization

#### File and Namespace Organization

**Organize code to reflect domain structure:**

```
// Domain-aligned namespace structure
 Prescriptions.Domain/
├── Entities/
│   ├── Patient.cs
│   ├── Prescription.cs
│   └── Pharmacy.cs
├── Primitives/
│   ├── PatientId.cs
│   ├── Money.cs
│   └── DrugCode.cs
├── Services/
│   ├── PrescriptionValidator.cs
│   └── EligibilityChecker.cs
└── Repositories/
    └── IPrescriptionRepository.cs
```

**Namespace naming should reflect business domains:**

```csharp
// GOOD: Business domain alignment
namespace  Prescriptions.Domain.Entities
namespace  Insurance.Domain.Services

// AVOID: Technical structure
namespace  Data.Models
namespace  Business.Logic
namespace  Common.Helpers
```

#### Class Structure for Discoverability

**Organize class members in a predictable order:**

```csharp
public class PrescriptionService
{
    // 1. Constants and static fields
    private const int MaxRetryAttempts = 3;

    // 2. Private fields
    private readonly IPrescriptionRepository _repository;
    private readonly IEligibilityChecker _eligibilityChecker;

    // 3. Constructor(s)
    public PrescriptionService(
        IPrescriptionRepository repository,
        IEligibilityChecker eligibilityChecker)
    {
        _repository = repository;
        _eligibilityChecker = eligibilityChecker;
    }

    // 4. Public methods (primary interface)
    public async Task<PrescriptionResult> CreatePrescription(CreatePrescriptionCommand command)
    {
        // Implementation
    }

    // 5. Private methods (implementation details)
    private async Task<ValidationResult> ValidateEligibility(PatientId patientId)
    {
        // Implementation
    }
}
```

#### Logical Grouping Patterns

**Group related functionality together:**

```csharp
public class Patient
{
    // Identity and basic info
    public PatientId Id { get; }
    public PatientName Name { get; }
    public DateOfBirth DateOfBirth { get; }

    // Insurance-related
    public Insurance PrimaryInsurance { get; private set; }
    public Insurance SecondaryInsurance { get; private set; }

    public void UpdatePrimaryInsurance(Insurance insurance) { }
    public void UpdateSecondaryInsurance(Insurance insurance) { }

    // Allergy-related
    private readonly List<Allergy> _allergies = new();

    public void AddAllergy(Allergy allergy) { }
    public void RemoveAllergy(AllergyId allergyId) { }
    public bool IsAllergicTo(Drug drug) { }

    // Prescription-related
    public bool IsEligibleFor(Drug drug) { }
    public Money CalculateCopayFor(Drug drug) { }
}
```

### Integration Points

#### DDD Ubiquitous Language Alignment

**Ensure code terminology matches domain expert language:**

```csharp
// Code should use the same terms that pharmacists, doctors, and insurance
// specialists use in their daily work

// GOOD: Matches healthcare terminology
public class PriorAuthorization
{
    public AuthorizationStatus Status { get; }
    public DateTime ExpirationDate { get; }

    public bool IsValidFor(Drug drug, Patient patient)
    public void Approve(PhysicianId approvingPhysician)
    public void Deny(DenialReason reason)
}

// AVOID: Technical terms not used by domain experts
public class PreAuthRecord
{
    public int StatusCode { get; }
    public DateTime EndDate { get; }

    public bool CheckValidity(object drugObj, object patientObj)
}
```

#### Cross-Reference with Existing Standards

**Self-documenting code supports other standards:**

- **Testing Standards**: Clear code makes tests easier to write and understand
- **Code Quality Standards**: Self-documenting code inherently follows SOLID principles
- **Domain-Driven Design**: Expressive types and names support rich domain models
- **Architectural Standards**: Clear interfaces and responsibilities support clean architecture

### Implementation Guidelines

#### Starting with Existing Code

**Gradually improve code documentation through refactoring:**

1. **Identify Primitive Obsession**: Look for `string`, `int`, `decimal` parameters that represent domain concepts
2. **Extract Domain Primitives**: Create specific types for domain concepts
3. **Improve Method Names**: Make method purposes clear from their names
4. **Eliminate Magic Values**: Replace with named constants or enums
5. **Simplify Method Responsibilities**: Break large methods into focused, single-purpose methods

### Code Review Checklist

**During code reviews, verify:**

- [ ] Class and method names clearly indicate their purpose
- [ ] Method parameters use domain-specific types, not primitives
- [ ] Business logic is expressed in domain terms
- [ ] Magic numbers and strings are replaced with named constants
- [ ] Comments explain "why" not "what"
- [ ] Error messages are specific and actionable
- [ ] Code structure reflects domain organization

## Conclusion

The code style is not just about writing clear names—it's about creating code that serves as the primary source of truth for how the system works. In healthcare domains where complexity is high and correctness is critical, self-documenting code becomes a safety mechanism that helps prevent misunderstandings and errors.

By following these standards, projects will have:
- Code that domain experts can read and validate
- Reduced onboarding time for new developers
- Fewer bugs caused by misunderstandings
- Living documentation that stays current with the code
- Improved collaboration between technical and business teams

The investment in writing self-documenting code pays dividends in maintainability, reliability, and team productivity throughout the lifecycle of the software.