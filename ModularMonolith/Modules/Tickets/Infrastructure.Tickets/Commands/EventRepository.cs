using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Event;
using Domain.Tickets.Venue;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Event.Event;

namespace Infrastructure.Tickets.Commands;

public class EventRepository(TicketDbContext ticketDbContext) : IPersistEvents
{
    public Task<Venue> GetByVenueId(Domain.ValueObjects.Venue venue)
    {
        return (ticketDbContext.Venues.FirstOrDefaultAsync(v => v.Id == venue) ?? throw new ValidationException($"Venue {venue} does not exist"))!;
    }

    public async Task Save(Event theEvent)
    {
        var @event = await GetById(theEvent.Id);
        
        if (@event is not null)
        {
            UpdateEvent(theEvent, @event);
            return;
        }

        AddEvent(theEvent);
    }

    private void AddEvent(Event theEvent)
    {
        ticketDbContext.Add(theEvent);
    }

    private void UpdateEvent(Event theEvent, Event @event)
    {
        @event.UpdateName(theEvent.EventName);
        @event.UpdateDates(theEvent.StartDate, theEvent.EndDate);
        @event.UpdateVenue(theEvent.Venue);
        @event.UpdatePrice(theEvent.Price);
        @event.TransferDomainEventsFrom(theEvent);
        ticketDbContext.Update(@event);
    }

    public async Task<Event?> GetById(Guid id)
    {
        return await ticketDbContext.Events
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}