using Application;
using Domain.Events.Contracts;
using Infrastructure.Events.Core;
using Messages.Events;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Events.Event;

public class EventRepository(EventDbContext eventDbContext, IPublishMessages publishEndpoint) : IPersistEvents
{
    public async Task Add(Domain.Events.Entities.Event theEvent)
    {
        eventDbContext.Add(theEvent);
        await publishEndpoint.Publish(new EventUpserted
        {
            Id = theEvent.Id, 
            EventName = theEvent.EventName,
            StartDate = theEvent.StartDate,
            EndDate = theEvent.EndDate,
            Venue = theEvent.Venue,
            Price = theEvent.Price
        });
    }

    public async Task Update(Domain.Events.Entities.Event @event)
    {
        eventDbContext.Update(@event);
        await publishEndpoint.Publish(new EventUpserted
        {
            Id = @event.Id, 
            EventName = @event.EventName,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Venue = @event.Venue,
            Price = @event.Price
        });
    }

    public async Task<Domain.Events.Entities.Event?> Get(Guid id)
    {
        return await eventDbContext.Events.FindAsync(id);
    }

    public async Task<IList<Domain.Events.Entities.Event>> GetAll()
    {
        return await eventDbContext.Events
            .Where(e => e.StartDate > DateTimeOffset.UtcNow)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }
}

