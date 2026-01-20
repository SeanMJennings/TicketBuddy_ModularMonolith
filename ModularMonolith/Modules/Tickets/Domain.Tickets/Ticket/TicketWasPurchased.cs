using Domain.DomainEvents;

namespace Domain.Tickets.Ticket;

public readonly record struct TicketWasPurchased(Guid TicketId, Guid UserId, Guid EventId) : IDescribeADomainEvent;