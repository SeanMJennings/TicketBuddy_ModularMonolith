using Application;
using Domain.Events.Venue;
using Infrastructure.Events.Core;
using Messages.Events;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Events.Venue;

public class VenueRepository(EventDbContext eventDbContext, IPublishMessages publishEndpoint) : IPersistVenues
{
    public async Task Add(Domain.Events.Venue.Venue venue)
    {
        eventDbContext.Add(venue);
        await publishEndpoint.Publish(new VenueUpserted
        {
            Id = venue.Id,
            Name = venue.Name,
            Capacity = venue.Capacity
        });
    }

    public async Task<Domain.Events.Venue.Venue?> GetById(Guid id)
    {
        return await eventDbContext.Venues.FindAsync(id);
    }

    public async Task<IEnumerable<Domain.Events.Venue.Venue>> GetAll()
    {
        return await eventDbContext.Venues.ToListAsync();
    }

    public async Task Update(Domain.Events.Venue.Venue venue)
    {
        eventDbContext.Update(venue);
        await publishEndpoint.Publish(new VenueUpserted
        {
            Id = venue.Id,
            Name = venue.Name,
            Capacity = venue.Capacity
        });
    }
}