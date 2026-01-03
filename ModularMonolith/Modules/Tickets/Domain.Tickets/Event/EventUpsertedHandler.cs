using Domain.DomainEvents;
using Domain.Tickets.Core;
using Domain.Tickets.Ticket;
using Domain.Tickets.Venue;

namespace Domain.Tickets.Event;

public class EventUpsertedHandler(
    IPersistTickets ticketsRepository,
    IPersistVenues venueRepository,
    ITicketsUnitOfWork unitOfWork) : HandleDomainEvents<EventUpserted>
{
    protected override async Task Handle(EventUpserted message)
    {
        var tickets = await ticketsRepository.GetByEventId(message.EventId);
        var venue = await venueRepository.GetById(message.VenueId);
        var ticketsHaveNotBeenReleased = tickets.Count == 0;
        
        if (ticketsHaveNotBeenReleased)
        {
            await TicketsReleaser.ReleaseTicketsForEvent(message.EventId, message.Price, venue!.Capacity,
                ticketsRepository, unitOfWork);
            return;
        }
            
        foreach (var ticket in tickets) ticket.UpdatePrice(message.Price);
        await ticketsRepository.UpdateRange(tickets);
        await unitOfWork.Commit();
    }
}