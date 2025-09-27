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

## Continuous Improvement

- **Code Reviews:** Go beyond finding bugs. Look for adherence to these principles, opportunities for simplification, and improvements in readability.
- **Automated Testing:** Use a mix of unit, integration, and end-to-end tests to lock in behavior and prevent regressions. High test coverage is a good indicator of maintainable code.
- **Documentation:** Document *why* decisions were made, not just *what* the code does. This is crucial for future maintainers.
