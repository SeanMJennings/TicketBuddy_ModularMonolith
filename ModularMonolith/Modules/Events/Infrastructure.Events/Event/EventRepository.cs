using Application;
using Domain.Events;
using Infrastructure.Events.Core;
using Messages.Events;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Events.Event;

public class EventRepository(EventDbContext eventDbContext, IPublishMessages publishEndpoint) : IPersistEvents
{
    public async Task Add(Domain.Events.Event theEvent)
    {
        eventDbContext.Add(theEvent);
        await PublishEventUpserted(theEvent);
    }

    public async Task Update(Domain.Events.Event @event)
    {
        eventDbContext.Update(@event);
        await PublishEventUpserted(@event);
    }

    private async Task PublishEventUpserted(Domain.Events.Event theEvent)
    {
        await publishEndpoint.Publish(new EventUpserted
        {
            Id = theEvent.Id,
            EventName = theEvent.EventName,
            StartDate = theEvent.StartDate,
            EndDate = theEvent.EndDate,
            VenueId = theEvent.VenueId,
            Price = theEvent.Price
        });
    }

    public async Task<Domain.Events.Event?> Get(Guid id)
    {
        return await eventDbContext.Events.FindAsync(id);
    }

    public async Task<IList<Domain.Events.Event>> GetAll()
    {
        return await eventDbContext.Events
            .Where(e => e.StartDate > DateTimeOffset.UtcNow)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }
}