using Domain.Tickets.Venue;
using Infrastructure.Tickets.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tickets.Venue;

public class VenueRepository(TicketDbContext ticketDbContext) : IPersistVenues
{
    public async Task Upsert(Domain.Tickets.Venue.Venue venue)
    {
        var existingVenue = await ticketDbContext.Venues
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == venue.Id);

        if (existingVenue is not null)
        {
            ticketDbContext.Update(venue);
            return;
        }

        ticketDbContext.Add(venue);
    }

    public async Task<Domain.Tickets.Venue.Venue?> GetById(Guid id)
    {
        return await ticketDbContext.Venues.FirstOrDefaultAsync(v => v.Id == id);
    }
}
