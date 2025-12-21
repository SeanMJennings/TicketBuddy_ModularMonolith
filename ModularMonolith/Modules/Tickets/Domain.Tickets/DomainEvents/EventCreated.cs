using Domain.DomainEvents;
using Domain.ValueObjects;

namespace Domain.Tickets.DomainEvents;

public readonly record struct EventCreated(Guid EventId, Money Price, int VenueCapacity) : IDescribeADomainEvent;