# Detailed Summary of Domain-Driven Design (DDD) by Eric Evans

This document provides an expanded summary of the core concepts from Eric Evans' book, "Domain-Driven Design: Tackling Complexity in the Heart of Software," with more detailed explanations, examples, and diagrams.

## Core Philosophy

Domain-Driven Design (DDD) is an approach to software development that prioritizes a deep understanding of the business domain. It advocates for creating a rich, expressive model of that domain and embedding it directly into the code. The central idea is that for most software projects, the primary complexity lies not in the technical challenges, but in the domain itself. By tackling this "essential complexity" head-on, teams can build more robust, maintainable, and valuable software.

---

## Part 1: The Building Blocks of a Model (Tactical Design)

### 1. Ubiquitous Language (In-Depth)

**Concept:** The Ubiquitous Language is the cornerstone of Domain-Driven Design. It is a shared, intentionally developed language that is used by everyone involved in the project—developers, domain experts, business analysts, and stakeholders. This language is not just a glossary of terms; it is a living, breathing part of the project that evolves as the team's understanding of the domain deepens. Its primary purpose is to bridge the communication gap between technical and non-technical team members, ensuring that the software model is a direct and accurate reflection of the business domain.

**Motivation:**
In many software projects, a "translation" process occurs between business requirements and technical implementation. Business experts describe their needs in their own language, and developers translate that into code, using their own technical jargon. This translation is a major source of errors, misunderstandings, and accidental complexity. The Ubiquitous Language eliminates this translation layer.

**The Problem of "Translation":**
- **Business says:** "We need to track our customers and their shipping details."
- **Developer hears:** "Create a `User` table with `address1`, `address2`, `city`, `zip` columns."
- **The disconnect:** The developer might miss the nuance that a "customer" only exists after their first purchase, or that "shipping details" can be different from their primary address. The model starts to diverge from the reality of the business from day one.

**The Ubiquitous Language Solution:**
The team collaborates to build a common language. They might decide that a `Prospect` becomes a `Customer` after a `Purchase` is made, and that a `Customer` has a `Profile` which contains a list of `ShippingDestinations`. These precise terms are then used by everyone, everywhere.

**Characteristics:**
- **Shared and Developed Collaboratively:** It is not imposed by one group on another. It is built through continuous conversation and collaboration between developers and domain experts.
- **Rich and Expressive:** It should be rich enough to describe the domain in detail, including its processes, rules, and constraints.
- **Context-Bound:** The language is specific to a single Bounded Context. The term "Policy" might mean one thing in an "Insurance" context and something completely different in a "Security" context.
- **Used Everywhere:** The commitment to the language must be absolute. It is used in:
    - **Code:** Class names, method names, variable names, module names.
    - **Databases:** Table and column names.
    - **User Interface:** Labels, buttons, and help text.
    - **Documentation:** Requirements, user stories, and technical diagrams.
    - **Conversations:** Team meetings, emails, and informal chats.

**Example in Practice:**
Consider a system for managing a shipping container terminal.
- **Initial, ambiguous term:** "Booking"
- **Through discussion, the team refines this into a Ubiquitous Language:**
    - A `Cargo` is registered for shipment.
    - A `Voyage` is a specific journey of a vessel.
    - A `RouteSpecification` defines the origin and destination.
    - A `HandlingEvent` (e.g., "Load", "Unload", "Claim") describes what happens to the `Cargo`.
    - A `CargoItinerary` is the plan for the `Cargo`, composed of one or more `Legs` (a `Voyage` from one location to another).

This refined language is then directly reflected in the code:
```csharp
public class Cargo
{
    public void SpecifyNewItinerary(RouteSpecification route) { ... }
    public void AssignToVoyage(Voyage voyage) { ... }
    public void RegisterHandlingEvent(HandlingEvent newEvent) { ... }
}
```
Now, the code reads like a clear statement of what the system does, understandable by both developers and domain experts.

**Benefits:**
- **Reduces Misunderstanding:** Eliminates the ambiguity that arises from translation between business and technical language.
- **Strengthens the Model:** A strong language leads to a strong, expressive domain model.
- **Improves Collaboration:** Fosters a deeper and more effective collaboration between all team members.
- **Makes the Code Self-Documenting:** The code itself becomes a clear and accurate description of the business domain.

### 2. Entities (In-Depth)

**Concept:** An Entity is a domain object characterized by a distinct and continuous identity. This identity runs through its entire lifecycle, from creation to deletion, and allows it to be distinguished from all other objects, even those with identical attributes. The attributes of an Entity can change over time, but its identity remains constant.

**Motivation:**
Many objects in a business domain are not simply defined by their properties; they have a history and a lifecycle that matters. For example, a person is still the same person even if they change their name or address. A specific product in a warehouse is still the same product even if its location changes. The Entity pattern allows us to model these core domain objects and track them uniquely through various state changes.

**Characteristics:**
-   **Identity:** The defining characteristic. The identity can be a natural key from the domain (like a vehicle's VIN or a person's social security number) but is more often a surrogate key (like a GUID or a database primary key) to ensure uniqueness and stability.
-   **Mutability:** Unlike Value Objects, Entities are typically mutable. Their attributes and relationships can change over their lifecycle. For example, a `Customer`'s `ShippingAddress` can be updated.
-   **Lifecycle:** Entities have a clear lifecycle that needs to be managed: they are created, loaded from persistence, modified through business operations, and eventually, may be deleted.
-   **Equality:** Equality for Entities is not based on their attributes but on their identity. Two `Customer` objects are considered the same if and only if they have the same `CustomerId`.

**Example: A Bank Account**
A `BankAccount` is a classic Entity. Its balance will change frequently, but it remains the same account because it has a unique `AccountNumber`.

```csharp
public class BankAccount
{
    public AccountNumber AccountNumber { get; private set; } // Identity
    public Money Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public BankAccount(AccountNumber accountNumber, Money initialDeposit)
    {
        AccountNumber = accountNumber;
        Balance = initialDeposit;
        Status = AccountStatus.Active;
    }

    public void Deposit(Money amount)
    {
        if (Status == AccountStatus.Frozen)
            throw new InvalidOperationException("Account is frozen.");
        Balance = Balance.Add(amount);
    }

    public void Withdraw(Money amount)
    {
        if (Status == AccountStatus.Frozen)
            throw new InvalidOperationException("Account is frozen.");
        if (Balance.Amount < amount.Amount)
            throw new InvalidOperationException("Insufficient funds.");
        Balance = Balance.Subtract(amount);
    }

    // Equality is based on the AccountNumber identity
    public override bool Equals(object obj)
    {
        var other = obj as BankAccount;
        return other != null && this.AccountNumber.Equals(other.AccountNumber);
    }
}
```
In this example, the `Balance` and `Status` can change, but the `AccountNumber` ensures we are always operating on the correct, unique account.

### 3. Value Objects (In-Depth)

**Concept:** A Value Object is an object that represents a descriptive aspect of the domain. Unlike an Entity, it has no conceptual identity and is defined entirely by its attributes. The core idea is to treat these objects like values, similar to how you would treat numbers or strings. If the value changes, you are conceptually dealing with a *different* object.

**Motivation:**
Many concepts in a domain do not have a unique identity. For example, the color "red" is defined by its RGB value, not by a unique ID. The amount "£10" is defined by its currency and quantity. Using primitive types (like `string` for an email address or `decimal` for a monetary amount) can lead to a loss of meaning and validation logic being scattered across the codebase. Value Objects solve this by creating small, cohesive objects that encapsulate the value and its associated business rules.

**Characteristics:**
- **No Identity:** A Value Object is identified by its value, not a unique ID. Equality is determined by comparing the attributes of the objects.
- **Immutability:** Once a Value Object is created, it should not be changed. If a change is needed, a new Value Object is created with the new value. This makes them thread-safe and predictable.
- **Descriptive:** They describe a characteristic of something else (often an Entity).
- **Self-Validating:** The constructor of a Value Object is the perfect place to enforce invariants and ensure that it can only be created in a valid state. For example, a `Money` object should not be created with a negative amount.
- **Composition:** They are often composed of other Value Objects.

**Example: Money**
Instead of passing `decimal` amounts around, a `Money` Value Object can encapsulate the amount and currency, along with relevant business logic.

```csharp
public class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Money amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency must be specified.");

        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        if (this.Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money of different currencies.");

        return new Money(this.Amount + other.Amount, this.Currency);
    }

    // Equality is based on attribute values
    public override bool Equals(object obj)
    {
        var other = obj as Money;
        return other != null &&
               this.Amount == other.Amount &&
               this.Currency == other.Currency;
    }
}
```

**Benefits:**
- **Increased Clarity and Expressiveness:** The code becomes more readable and self-documenting. `Money` is much clearer than `decimal`.
- **Reduced Primitive Obsession:** It helps avoid the "primitive obsession" anti-pattern, where the codebase is filled with primitive types that have implicit meaning.
- **Centralized Business Logic:** Business rules related to the value (e.g., validation, calculations) are encapsulated within the Value Object itself, rather than being scattered across the application.
- **Improved Safety and Robustness:** Immutability and self-validation make the code safer and less prone to bugs. You can be confident that a Value Object, once created, is always in a valid state.

### 4. Aggregates (In-Depth)

**Concept:** An Aggregate is a pattern for defining ownership and boundaries among a cluster of related domain objects. It groups Entities and Value Objects that must remain consistent with each other into a single unit, which is then treated as a whole for the purpose of data changes. The goal is to maintain the integrity of the model by enforcing business rules, known as **invariants**, within this boundary.

**Motivation:**
In any complex domain, objects are rarely independent. For example, an `Order` has `OrderLines`, and the total price of the `Order` must equal the sum of the prices of its `OrderLines`. If you allowed a developer to add an `OrderLine` without updating the `Order`'s total price, the model would become inconsistent. Aggregates prevent this by defining a clear boundary and a single point of entry for all modifications.

**Rules of Aggregates:**
1.  **The Aggregate Root:** Every Aggregate has a single Entity that serves as its root. This **Aggregate Root** is the public face of the Aggregate.
2.  **The Consistency Boundary:** The boundary of the Aggregate defines what must be consistent at all times. Any business rule that spans multiple objects within the Aggregate must be enforced by the Aggregate Root.
3.  **External References:** External objects are only allowed to hold a reference to the Aggregate Root. They cannot hold references to any other object inside the Aggregate. This prevents external objects from making changes that could violate the Aggregate's invariants.
4.  **Transactional Integrity:** All objects within an Aggregate are loaded and saved together as a single atomic unit.

**Diagram:**
```
+-------------------------------------------------+
|                 PurchaseOrder Aggregate         |
|                                                 |
|   +-----------------------------------------+   |
|   |      PurchaseOrder (Aggregate Root)     |   |  <-- External Reference
|   |-----------------------------------------|   |
|   | - Id                                    |   |
|   | - VendorInfo                            |   |
|   | - TotalPrice                            |   |
|   | - Status                                |   |
|   |                                         |   |
|   |  +-----------------+  +-----------------+   |
|   |  | OrderLine (Entity)|  | OrderLine (Entity)|   |  <-- No External References
|   |  +-----------------+  +-----------------+   |
|   |                                         |   |
|   |  Methods to enforce invariants:         |   |
|   |   - AddItem(...)                        |   |
|   |   - ChangeQuantity(...)                 |   |
|   |   - Submit()                            |   |
|   +-----------------------------------------+   |
|                                                 |
+-------------------------------------------------+  <-- Transactional Boundary
```

**Example:**
A `PurchaseOrder` is the Aggregate Root. It contains a list of `OrderLine` Entities. To add an item, you must call a method on the `PurchaseOrder` itself.

```csharp
public class PurchaseOrder
{
    public Guid Id { get; private set; }
    private readonly List<OrderLine> _orderLines = new List<OrderLine>();
    public Money TotalPrice { get; private set; }

    public void AddItem(Product product, int quantity)
    {
        if (Status == OrderStatus.Submitted)
            throw new InvalidOperationException("Cannot add items to a submitted order.");

        var line = new OrderLine(product.Id, quantity, product.Price);
        _orderLines.Add(line);

        // Enforce the invariant: TotalPrice must always be correct.
        RecalculateTotalPrice();
    }

    private void RecalculateTotalPrice()
    {
        TotalPrice = _orderLines.Aggregate(Money.Zero(), (total, line) => total.Add(line.Price));
    }
}
```
By forcing all changes to go through the `PurchaseOrder`'s methods, we guarantee that the `TotalPrice` is always consistent with the `OrderLines`.

### 5. Repositories (In-Depth)

**Concept:** A Repository is a design pattern that mediates between the domain and data mapping layers. It provides a collection-like interface for accessing domain objects (specifically, Aggregate Roots), completely hiding the details of the underlying persistence mechanism. From the perspective of the domain model, the data appears to be in an in-memory collection.

**Motivation:**
The domain model should be focused on business logic, not on the technical details of how data is stored or retrieved. Sprinkling data access code (like SQL queries or ORM-specific code) throughout the domain layer would make it complex, hard to test, and tightly coupled to a specific database technology. Repositories provide a clean separation of concerns, keeping the domain pure and infrastructure-agnostic.

**Characteristics:**
-   **One Repository per Aggregate:** There is typically a one-to-one relationship between an Aggregate type and a Repository. You have an `OrderRepository` for the `Order` Aggregate, a `CustomerRepository` for the `Customer` Aggregate, and so on. You do not have repositories for other objects within the aggregate.
-   **Collection-like Interface:** The interface of a Repository should look and feel like a simple collection. Common methods include `Add`, `Remove`, `FindById`, and `FindAll`. The methods should be expressed in terms of the Ubiquitous Language.
-   **Hides Persistence Logic:** The implementation of the Repository (the concrete class) contains all the infrastructure-specific code for interacting with the database. The domain layer only knows about the interface.
-   **Manages the Object Lifecycle:** The Repository is responsible for retrieving a complete, consistent Aggregate from the database and for persisting its state back to the database.

**Example: An `ICustomerRepository`**
The interface is defined in the domain layer and is simple and expressive.

```csharp
// In the Domain Layer
public interface ICustomerRepository
{
    Customer GetById(Guid customerId);
    void Add(Customer customer);
    void Update(Customer customer); // Or just a Save(Customer) method
}
```

The implementation resides in the infrastructure layer and handles the database-specific details.

```csharp
// In the Infrastructure Layer
public class CustomerRepository : ICustomerRepository
{
    private readonly MyDbContext _dbContext;

    public CustomerRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Customer GetById(Guid customerId)
    {
        // Code to retrieve the customer and all its related objects (the full aggregate)
        return _dbContext.Customers.Include(c => c.Addresses).Single(c => c.Id == customerId);
    }

    public void Add(Customer customer)
    {
        _dbContext.Customers.Add(customer);
    }

    public void Update(Customer customer)
    {
        // EF Core can track changes, so this might be empty,
        // and the work is done by a Unit of Work pattern.
    }
}
```
The application service would use the repository like this:
`var customer = _customerRepository.GetById(id);`
It has no idea whether the data is coming from SQL Server, a file, or a web service.

### 6. Factories (In-Depth)

**Concept:** A Factory is a domain object whose primary responsibility is to create other domain objects, particularly complex Entities or entire Aggregates. It encapsulates the logic of creation, ensuring that all invariants are met and the resulting object is in a consistent, valid state from the moment it comes into existence.

**Motivation:**
The creation of a domain object can sometimes be a significant operation in itself. The logic might be too complex or out of place for a simple constructor. For example, creating an object might require:
-   Accessing a Repository to check for duplicates or assign a sequence number.
-   Calling an external service (e.g., a credit check service).
-   Complex business rules to determine the initial state.
-   Creating a full Aggregate, not just the root Entity.

Placing this logic in a constructor would violate the Single Responsibility Principle and couple the domain object to infrastructure concerns (like repositories or services). Factories provide a clean way to handle this complexity.

**Characteristics:**
-   **Encapsulates Creation Logic:** A Factory's methods hide the details of how an object is created. The client code simply asks the factory for an object, providing the necessary information.
-   **Atomic Creation:** The Factory should create the object or Aggregate in its entirety and ensure it is valid. It should not return a half-built object.
-   **Abstracts the Concrete Type:** A Factory can be used to create an instance of a concrete class that implements an interface, allowing the client to be unaware of the specific implementation.
-   **Can be a Method or a Class:** A Factory can be a standalone class (e.g., `CustomerFactory`) or a method on an existing Aggregate Root that is responsible for creating child objects within that Aggregate.

**Example: Creating a `ForumThread`**
Creating a new thread on a forum might require checking the user's permissions and assigning a unique ID.

```csharp
public class ForumThreadFactory
{
    private readonly IUserRepository _userRepository;
    private readonly IThreadRepository _threadRepository;

    public ForumThreadFactory(IUserRepository userRepository, IThreadRepository threadRepository)
    {
        _userRepository = userRepository;
        _threadRepository = threadRepository;
    }

    public ForumThread CreateNewThread(Guid authorId, string title, string initialPostContent)
    {
        var author = _userRepository.GetById(authorId);
        if (!author.CanPostNewThreads())
        {
            throw new PermissionDeniedException("User is not allowed to post new threads.");
        }

        var threadId = _threadRepository.GetNextId();
        var thread = new ForumThread(threadId, author.Id, title, initialPostContent);

        // The factory ensures the thread is created in a valid state
        // with a valid author and a unique ID.
        return thread;
    }
}
```
Here, the Factory handles the business rules and infrastructure lookups (repositories) that are part of the creation process, keeping the `ForumThread`'s constructor clean and focused on simply assigning values.

---

## Part 2: The Lifecycle of a Model (Strategic Design)

### 7. Bounded Context (In-Depth)

**Concept:** A Bounded Context is a central pattern in DDD that defines the explicit boundary within which a particular domain model is consistent and applicable. It acts as a conceptual frame, ensuring that every term, concept, and rule within that frame has a single, unambiguous meaning. This is crucial for managing complexity in large systems where a single, unified model of the entire business is impractical and often impossible.

**Motivation:**
In any large organization, different departments or teams often have different perspectives and use different terminology, even when talking about the same conceptual thing. For example:
- To the **sales team**, a "Product" is something with a price, description, and discount policy.
- To the **warehouse team**, a "Product" is a physical item with a weight, dimensions, stock level, and location.
- To the **customer support team**, a "Product" is something with a warranty, user manual, and a list of common issues.

Attempting to create a single `Product` class that serves all these needs would result in a bloated, complex, and fragile object. The Bounded Context pattern solves this by acknowledging that these are different models of a "Product," each valid within its own context.

**Characteristics:**
- **Explicit Boundary:** The team defines a clear boundary for the context. This boundary is often aligned with team organization, a specific business capability, or a particular part of the codebase (like a microservice).
- **Unified Model:** Inside the boundary, the domain model is unified and consistent. There is one and only one definition for each term in the Ubiquitous Language.
- **Linguistic Integrity:** The Bounded Context protects the meaning of the Ubiquitous Language, preventing it from becoming diluted or ambiguous.
- **Autonomous Development:** Teams can develop and evolve their Bounded Contexts independently, as long as they respect the contracts for integration with other contexts.

**Diagram:**
The diagram illustrates how the concept of a "Product" is modeled differently in two distinct Bounded Contexts:

```
+--------------------------------------+         +--------------------------------------+
|       Sales Bounded Context          |         |      Inventory Bounded Context       |
| (Focus: Selling Products)            |         | (Focus: Storing & Tracking Products) |
|--------------------------------------|         |--------------------------------------|
|                                      |         |                                      |
|  Model of "Product":                 |         |  Model of "Product":                 |
|   - ProductId (SKU)                  |         |   - ProductId (SKU)                  |
|   - Price                            |         |   - StockLevel                       |
|   - MarketingDescription             |         |   - WarehouseLocation                |
|   - ApplicableDiscounts              |         |   - Weight                           |
|   - CustomerReviews                  |         |   - SupplierInfo                     |
|                                      |         |                                      |
+--------------------------------------+         +--------------------------------------+
```
In this example, `ProductId` (or SKU) acts as a shared identifier that allows the system to correlate the different representations of the product across contexts.

**Benefits:**
- **Reduces Complexity:** By breaking down a large, complex domain into smaller, more manageable models, Bounded Contexts make the system easier to understand, reason about, and maintain.
- **Improves Team Autonomy:** Different teams can work on different Bounded Contexts with a high degree of autonomy, reducing development bottlenecks.
- **Enables Clear Integration:** By making the boundaries explicit, DDD forces teams to think carefully about how different parts of the system should interact, leading to cleaner and more intentional integration patterns (as defined in the Context Map).
- **Preserves Model Integrity:** Each model is protected from the influence of other, unrelated concepts, ensuring its internal consistency and alignment with its specific part of the business domain.

### 8. Context Map (In-Depth)

**Concept:** A Context Map is a crucial tool for strategic design in DDD. It is a document or diagram that identifies the different Bounded Contexts in a system and clarifies the relationships between them. It provides a holistic view of the solution landscape, making the boundaries and the points of integration explicit.

**Motivation:**
Once you have identified your Bounded Contexts, you need to manage how they interact. A Context Map is the tool for this. It helps teams to negotiate and define the nature of their collaboration and technical integration. Without a Context Map, integrations often happen in an ad-hoc way, leading to tangled dependencies and a "Big Ball of Mud" architecture.

**Common Relationship Patterns:**
The Context Map uses a set of named patterns to describe the relationship between any two Bounded Contexts. The "Upstream" context is the one being consumed, and the "Downstream" context is the consumer.

-   **Partnership:** Two contexts are mutually dependent. The teams must collaborate closely and succeed or fail together.
-   **Shared Kernel:** Two teams agree to share a common, small subset of their domain models (e.g., a set of Value Objects). This requires a high degree of collaboration, as changes to the kernel affect both teams.
-   **Customer-Supplier:** A classic upstream/downstream relationship. The downstream team is a "customer" of the upstream team. The needs of the downstream team are a major driver for the upstream team's planning.
-   **Conformist:** The downstream team chooses to conform to the model of the upstream team. They adopt the upstream's Ubiquitous Language and model without trying to translate it. This is a simple approach but makes the downstream context highly dependent on the upstream one.
-   **Anti-Corruption Layer (ACL):** The downstream team builds a defensive layer that translates the upstream context's model into a model that is more suitable for its own Bounded Context. This isolates the downstream context from changes in the upstream model, preventing its own language and model from being "corrupted." This is a very common and powerful pattern.
-   **Open-Host Service:** The upstream context defines a clear, well-documented protocol (like a REST API with a formal specification) that is open for any downstream context to consume. The upstream team's goal is to provide a stable and reliable service.
-   **Separate Ways:** The two contexts have no integration at all. This is the simplest solution when integration is not truly necessary.

**Example Diagram:**
```
                               +-----------------+
                               |  Shared Kernel  |
                               | (Core Types)    |
                               +-------+---------+
                                       |
                      +----------------+----------------+
                      |                                 |
+---------------------v-+         +---------------------v-+
|  Sales Context      +<--------->+  Marketing Context  | (Partnership)
| (Upstream)          |           | (Upstream)          |
+---------------------+           +---------------------+
        | (Open-Host Service)           | (Conformist)
        |                               |
+-------v-------------------------------+
|       Billing Context (Downstream)    |
|                                       |
|  +---------------------------------+  |
|  |   Anti-Corruption Layer (ACL)   |  |
|  +---------------------------------+  |
|  |      Internal Billing Model     |  |
|  +---------------------------------+  |
+---------------------------------------+
```
This map clearly shows that Sales and Marketing are partners sharing a kernel. The Billing context consumes both, but it protects its own model from the Sales context with an ACL, while simply conforming to the model of the Marketing context.

### 9. Domain Events (In-Depth)

**Concept:** A Domain Event is a domain object that represents something significant that has occurred in the past within a Bounded Context. It is a record of a business fact. Domain Events are a key pattern for modeling side effects and for communicating between different parts of a system (or between different Bounded Contexts) in a loosely coupled way.

**Motivation:**
Many business processes have side effects. When an order is placed, it might trigger a notification to the customer, an update to the inventory, and an entry in the shipping manifest. Tightly coupling all this logic into a single `PlaceOrder` method would create a complex and brittle system. Domain Events allow you to reverse this dependency. The `PlaceOrder` method simply does its core job and then announces what happened by publishing an `OrderPlaced` event. Other parts of the system can then react to this event as needed.

**Characteristics:**
-   **Named in the Past Tense:** An event represents something that has already happened, so it should be named accordingly (e.g., `OrderConfirmed`, `PasswordChanged`, `ShipmentDispatched`).
-   **Immutable:** Because an event is a record of the past, it should be immutable.
-   **Carries Context:** The event object should contain all the necessary information for a listener to understand what happened without having to query the original system. For example, an `OrderConfirmed` event should contain the `OrderId`, `CustomerId`, and `OrderDate`.
-   **Enables Loose Coupling:** The publisher of an event does not know or care about the subscribers. This allows you to add new subscribers and new business processes without changing the original code.
-   **Facilitates Eventual Consistency:** Domain Events are a natural fit for asynchronous communication and are often used in systems that embrace eventual consistency. The state of the overall system becomes consistent over time as different subscribers process the events.

**Example: The `OrderConfirmed` Event**
1.  A user confirms their order in the **Sales Bounded Context**.
2.  The `Order` Aggregate processes the confirmation and, as part of the transaction, creates and publishes an `OrderConfirmed` event.
    ```csharp
    public class Order
    {
        public void Confirm()
        {
            this.Status = OrderStatus.Confirmed;
            // Add the event to a collection to be published after the transaction commits
            this.AddDomainEvent(new OrderConfirmed(this.Id, this.CustomerId, this.OrderDate));
        }
    }
    ```
3.  An event bus or message queue picks up this event and distributes it.
4.  Multiple subscribers in different Bounded Contexts react to the event:
    -   **The Notifications Context** listens for `OrderConfirmed` and sends a confirmation email to the customer.
    -   **The Inventory Context** listens for `OrderConfirmed` and decrements the stock level for the ordered products.
    -   **The Shipping Context** listens for `OrderConfirmed` and creates a new shipment record, putting it in the "Awaiting Pickup" state.

Each of these contexts is completely decoupled from the Sales context and from each other. They can be developed, deployed, and scaled independently.

## Conclusion

Domain-Driven Design provides a robust framework for building software that solves complex business problems. By focusing on the domain, using a Ubiquitous Language, and applying both tactical and strategic design patterns, teams can create systems that are not only technically sound but also deeply aligned with the business they serve.
