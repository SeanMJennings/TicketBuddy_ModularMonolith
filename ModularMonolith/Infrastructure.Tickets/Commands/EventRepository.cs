using Domain.Tickets.Contracts;
using Domain.Tickets.DomainEvents;
using Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Tickets.Entities.Event;

namespace Infrastructure.Tickets.Commands;

public class EventRepository(TicketDbContext ticketDbContext) : IAmAnEventRepository
{
    public async Task Save(Event theEvent)
    {
        var @event = await Get(theEvent.Id);
        
        if (@event is not null)
        {
            @event.UpdateName(theEvent.EventName);
            @event.UpdateDates(theEvent.StartDate, theEvent.EndDate);
            @event.UpdateVenue(theEvent.Venue);
            @event.UpdatePrice(theEvent.Price);
            
            @event.AddDomainEvent(new EventUpserted
            {
                Id = theEvent.Id,
                Venue = theEvent.Venue,
                Price = theEvent.Price
            });
            
            ticketDbContext.Update(@event);
        }
        else
        {
            theEvent.AddDomainEvent(new EventUpserted
            {
                Id = theEvent.Id,
                Venue = theEvent.Venue,
                Price = theEvent.Price
            });
            
            ticketDbContext.Add(theEvent);
        }
    }

    public async Task<Venue> GetVenue(Domain.Events.Primitives.Venue venue)
    {
        return await ticketDbContext.Venues.FirstAsync(v => v.Id == venue);
    }

    private async Task<Event?> Get(Guid id)
    {
        return await ticketDbContext.Events.FindAsync(id);
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
}