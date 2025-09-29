using Domain.DomainEvents;

namespace Domain.Tickets.DomainEvents;

public readonly record struct AllTicketsSold(Guid EventId) : IAmADomainEvent;
