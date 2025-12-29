using Domain.DomainEvents;

namespace Domain.Tickets.Ticket;

public readonly record struct AllTicketsSold(Guid EventId) : IDescribeADomainEvent;