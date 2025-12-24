using Domain.DomainEvents;
using Domain.Tickets.Contracts;
using Domain.Tickets.DomainEvents;
using Domain.Tickets.Services;

namespace Domain.Tickets.DomainEventHandlers;

public class EventUpsertedHandler(IPersistEvents eventsRepository, IPersistTickets ticketsRepository, ITicketsUnitOfWork unitOfWork) : HandleDomainEvents<EventUpserted>
{
    protected override async Task Handle(EventUpserted message)
    {
        var tickets = await ticketsRepository.GetByEventId(message.EventId);
        var venue = await eventsRepository.GetByVenueId(message.Venue);
        var ticketsHaveNotBeenReleased = tickets.Count == 0;
        
        if (ticketsHaveNotBeenReleased)
        {
            await TicketsReleaser.ReleaseTicketsForEvent(message.EventId, message.Price, venue.Capacity,
                ticketsRepository, unitOfWork);
            return;
        }
            
        foreach (var ticket in tickets) ticket.UpdatePrice(message.Price);
        await ticketsRepository.UpdateRange(tickets);
        await unitOfWork.Commit();
    }
}
