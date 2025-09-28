using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Entities.Event;

namespace Infrastructure.Tickets.Commands;

public class EventRepository(TicketDbContext ticketDbContext) : IAmAnEventRepository
{
    public Task<Venue> GetByVenueId(Domain.Events.Primitives.Venue venue)
    {
        return (ticketDbContext.Venues.FirstOrDefaultAsync(v => v.Id == venue) ?? throw new ValidationException($"Venue {venue} does not exist"))!;
    }

    public async Task Save(Event theEvent)
    {
        var @event = await GetById(theEvent.Id);
        
        if (@event is not null)
        {
            @event.UpdateName(theEvent.EventName);
            @event.UpdateDates(theEvent.StartDate, theEvent.EndDate);
            @event.UpdateVenue(theEvent.TheVenue);
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
}