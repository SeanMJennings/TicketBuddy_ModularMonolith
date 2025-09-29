using Domain.Events.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Event = Domain.Events.Entities.Event;

namespace Infrastructure.Events.Persistence;

public class EventRepository(EventDbContext eventDbContext, IPublishEndpoint publishEndpoint) : IAmAnEventRepository
{
    public async Task Add(Event theEvent)
    {
        eventDbContext.Add(theEvent);
        await publishEndpoint.Publish(new Integration.Events.Messaging.EventUpserted
        {
            Id = theEvent.Id, 
            EventName = theEvent.EventName,
            StartDate = theEvent.StartDate,
            EndDate = theEvent.EndDate,
            Venue = theEvent.Venue,
            Price = theEvent.Price
        });
    }

    public async Task Update(Event @event)
    {
        eventDbContext.Update(@event);
        await publishEndpoint.Publish(new Integration.Events.Messaging.EventUpserted
        {
            Id = @event.Id, 
            EventName = @event.EventName,
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            Venue = @event.Venue,
            Price = @event.Price
        });
    }

    public async Task<Event?> Get(Guid id)
    {
        return await eventDbContext.Events.FindAsync(id);
    }

    public async Task<IList<Event>> GetAll()
    {
        return await eventDbContext.Events
            .Where(e => e.StartDate > DateTimeOffset.UtcNow)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await eventDbContext.Commit(cancellationToken);
    }
}