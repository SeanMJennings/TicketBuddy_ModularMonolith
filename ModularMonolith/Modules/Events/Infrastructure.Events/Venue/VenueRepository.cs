using Domain.Events.Venue;
using Infrastructure.Events.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Events.Venue;

public class VenueRepository(EventDbContext eventDbContext) : IPersistVenues
{
    public void Add(Domain.Events.Venue.Venue venue)
    {
        eventDbContext.Add(venue);
    }

    public async Task<Domain.Events.Venue.Venue?> GetById(Guid id)
    {
        return await eventDbContext.Venues.FindAsync(id);
    }

    public async Task<IEnumerable<Domain.Events.Venue.Venue>> GetAll()
    {
        return await eventDbContext.Venues.ToListAsync();
    }
}