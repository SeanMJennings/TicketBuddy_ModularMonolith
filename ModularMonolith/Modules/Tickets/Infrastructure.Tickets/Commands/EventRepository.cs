using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Entities.Event;

namespace Infrastructure.Tickets.Commands;

public class EventRepository(TicketDbContext ticketDbContext) : IPersistEvents
{
    public Task<Venue> GetByVenueId(Domain.Primitives.Venue venue)
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
        theEvent.ReleaseNewTickets();
        ticketDbContext.Add(theEvent);
    }

    private void UpdateEvent(Event theEvent, Event @event)
    {
        @event.UpdateName(theEvent.EventName);
        @event.UpdateDates(theEvent.StartDate, theEvent.EndDate);
        @event.UpdateVenue(theEvent.TheVenue);
        @event.UpdatePrice(theEvent.Price);
        var updatedTicketIds = @event.UpdateExistingTicketsThatAreNotPurchased();

        foreach (var ticket in @event.Tickets.Where(t => updatedTicketIds.Contains(t.Id)))
        {
            ticketDbContext.Entry(ticket).State = EntityState.Modified;
        }
            
        ticketDbContext.Update(@event);
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
}