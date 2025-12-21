using Domain.DomainEvents;
using Domain.Tickets.Contracts;
using Domain.Tickets.DomainEvents;
using Domain.Tickets.Services;

namespace Domain.Tickets.DomainEventHandlers;

public class EventCreatedHandler(IPersistTickets persistTickets) : HandleDomainEvents<EventCreated>
{
    protected override async Task Handle(EventCreated message)
    {
        await TicketsReleaser.ReleaseTicketsForEvent(message.EventId, message.Price, message.VenueCapacity, persistTickets);
    }
}
