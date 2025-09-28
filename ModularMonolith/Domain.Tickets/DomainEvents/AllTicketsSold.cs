using Domain.DomainEvents;

namespace Domain.Tickets.DomainEvents;

public record AllTicketsSold(Guid EventId) : IAmADomainEvent;
