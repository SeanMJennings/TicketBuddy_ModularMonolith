using Domain.Tickets.Event;
using Infrastructure.Tickets.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tickets.Event;

public class EventRepository(TicketDbContext ticketDbContext) : IPersistEvents
{
    public async Task Upsert(Domain.Tickets.Event.Event theEvent)
    {
        var @event = await GetById(theEvent.Id);
        
        if (@event is not null)
        {
            UpdateEvent(theEvent, @event);
            return;
        }

        AddEvent(theEvent);
    }

    private void AddEvent(Domain.Tickets.Event.Event theEvent)
    {
        ticketDbContext.Add(theEvent);
    }

    private void UpdateEvent(Domain.Tickets.Event.Event theEvent, Domain.Tickets.Event.Event @event)
    {
        @event.UpdateName(theEvent.EventName);
        @event.UpdateDates(theEvent.StartDate, theEvent.EndDate);
        @event.UpdateVenue(theEvent.VenueId);
        @event.UpdatePrice(theEvent.Price);
        @event.TransferDomainEventsFrom(theEvent);
        ticketDbContext.Update(@event);
    }

    public async Task<Domain.Tickets.Event.Event?> GetById(Guid id)
    {
        return await ticketDbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}

