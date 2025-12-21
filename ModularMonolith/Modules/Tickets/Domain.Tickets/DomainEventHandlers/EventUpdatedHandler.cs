using Domain.DomainEvents;
using Domain.Tickets.Contracts;
using Domain.Tickets.DomainEvents;

namespace Domain.Tickets.DomainEventHandlers;

public class EventUpdatedHandler(IPersistTickets ticketsRepository) : HandleDomainEvents<EventUpdated>
{
    protected override async Task Handle(EventUpdated message)
    {
        var tickets = await ticketsRepository.GetByEventId(message.EventId);
            
        foreach (var ticket in tickets)
        {
            ticket.UpdatePrice(message.Price);
        }

        await ticketsRepository.UpdateRange(tickets);
    }
}
