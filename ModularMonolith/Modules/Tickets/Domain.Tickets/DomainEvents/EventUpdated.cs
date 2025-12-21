using Domain.DomainEvents;
using Domain.ValueObjects;

namespace Domain.Tickets.DomainEvents;

public readonly record struct EventUpdated(Guid EventId, Money Price) : IDescribeADomainEvent;