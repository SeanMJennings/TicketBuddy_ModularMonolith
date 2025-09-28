using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Entities.Event;

namespace Infrastructure.Tickets.Commands;

public class EventRepository(TicketDbContext ticketDbContext) : IAmAnEventRepository
{
    public async Task Save(Event theEvent)
    {
        var @event = await GetById(theEvent.Id);
        
        if (@event is not null)
        {
            @event.UpdateName(theEvent.EventName);
            @event.UpdateDates(theEvent.StartDate, theEvent.EndDate);
            @event.UpdateVenue(theEvent.Venue);
            @event.UpdatePrice(theEvent.Price);
            @event.UpdateExistingTicketsThatAreNotPurchased();
            ticketDbContext.Update(@event);
            foreach (var ticket in @event.Tickets)
            {
                ticketDbContext.Entry(ticket).State = EntityState.Modified;
            }
        }
        else
        {
            var theVenue = await GetVenueForEvent(theEvent);
            theEvent = theEvent.ToEvent(theVenue);
            theEvent.ReleaseNewTickets();
            ticketDbContext.Add(theEvent);
        }
    }

    public async Task<Event?> GetById(Guid Id)
    {
        return await ticketDbContext.Events
            .Include(e => e.TheVenue)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == Id);
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
    
    private async Task<Venue> GetVenueForEvent(Event theEvent)
    {
        var venue = await ticketDbContext.Venues.FirstOrDefaultAsync(v => v.Id == theEvent.Venue);
        return venue ?? throw new ValidationException($"Venue {theEvent.Venue} does not exist");
    }
}

public static class EventConverter
{
    public static Event ToEvent(this Event theEvent, Venue venue)
    {
        return new Event(theEvent.Id, theEvent.EventName, theEvent.StartDate, theEvent.EndDate, venue, theEvent.Price);
    }
}