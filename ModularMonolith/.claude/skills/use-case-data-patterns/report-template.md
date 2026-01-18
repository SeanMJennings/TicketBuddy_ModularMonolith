# Report Template

Complete structure for use case analysis reports.

```markdown
# Use Case Analysis Report

## Use Case Summary
[Brief description of the use case in 2-3 sentences]

## Architecture Flow
[Step-by-step trace through the system]
1. Entry Point: [endpoint, route, handler]
2. Middleware/Guards: [relevant middleware, guards, interceptors]
3. Business Logic: [services, use cases, domain models]
4. Data Access: [database queries, repositories, caching]
5. External Integrations: [third-party APIs, message queues, events]

## Data Access Patterns

### Database Patterns
[Specific queries, tables, models, schemas, migrations involved]

### Caching Patterns
[Cache usage, cache keys, TTL strategies, invalidation patterns]

### External Integration Patterns
[How data flows to/from external services, APIs, message queues]

## Relevant Code Locations
- Entry Points: [file paths to controllers, handlers, routes]
- Business Logic: [file paths to services, use cases, domain models]
- Abstractions: [file paths to interfaces, abstract classes]
- Implementations: [file paths to concrete implementations]
- Data Layer: [file paths to repositories, models, migrations]
- Transformations: [file paths to mappers, DTOs, serializers]

## Current Implementation Status
[What exists, what works, what's complete]

## Gaps and Missing Patterns
[What's missing or incomplete]

## Recommendations
[Specific suggestions for completing the data access patterns]
1. [Recommendation with rationale]
2. [Recommendation with rationale]

## Notes
[Any additional context, edge cases, or considerations]
```

## Example Report

```markdown
# Use Case Analysis Report

## Use Case Summary
Users can view and filter a list of orders by date range and status. The system displays order details including items, totals, and shipping information.

## Architecture Flow
1. Entry Point: `GET /api/orders` → `OrdersController.GetOrders()`
2. Middleware/Guards: `AuthenticationMiddleware`, `RateLimitingMiddleware`
3. Business Logic: `OrderQueryService.GetFilteredOrders()`
4. Data Access: `OrderRepository.FindByFilters()` → EF Core query
5. External Integrations: None for this use case

## Data Access Patterns

### Database Patterns
- Table: `Orders` with columns (Id, UserId, Status, CreatedAt, Total)
- Table: `OrderItems` with FK to Orders
- Query: Filtered by UserId, Status, DateRange with pagination
- Index: `IX_Orders_UserId_CreatedAt` for efficient filtering

### Caching Patterns
- No caching currently implemented for order lists
- Individual order details cached with key `order:{id}` (5 min TTL)

### External Integration Patterns
- N/A for this read operation

## Relevant Code Locations
- Entry Points: `src/Api/Controllers/OrdersController.cs:45`
- Business Logic: `src/Orders/Services/OrderQueryService.cs:23`
- Abstractions: `src/Orders/Ports/IOrderRepository.cs`
- Implementations: `src/Orders/Adapters/OrderRepository.cs:67`
- Data Layer: `src/Orders/Models/Order.cs`, `src/Migrations/20250115_AddOrders.cs`
- Transformations: `src/Orders/Mappers/OrderMapper.cs`

## Current Implementation Status
- ✅ Basic filtering by status works
- ✅ Pagination implemented
- ⚠️ Date range filtering exists but not optimized
- ❌ No sorting options implemented

## Gaps and Missing Patterns
1. Missing index for date range queries (causes table scan)
2. No caching for frequently accessed filter combinations
3. Sorting not implemented in repository layer

## Recommendations
1. Add composite index on (UserId, CreatedAt, Status) for common filter patterns
2. Implement query result caching for common filter combinations
3. Add OrderBy parameter to repository method signature

## Notes
- Consider adding a read model if order list queries become a bottleneck
- Current N+1 issue when loading OrderItems - consider eager loading
```