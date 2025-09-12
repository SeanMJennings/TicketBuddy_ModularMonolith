using Domain.DomainEvents;
using Domain.Events.Primitives;

namespace Domain.Tickets.DomainEvents;

public record EventUpserted : IAmADomainEvent
{
    public Guid Id { get; init; }
    public Venue Venue { get; init; }
    public decimal Price { get; init; }
}