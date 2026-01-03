using Domain.DomainEvents;
using Domain.ValueObjects;

namespace Domain.Tickets.Event;

public readonly record struct EventUpserted(Guid EventId, Money Price, Guid VenueId) : IDescribeADomainEvent;