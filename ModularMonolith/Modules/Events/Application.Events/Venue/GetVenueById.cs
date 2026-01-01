using Domain.Events.Venue;

namespace Application.Events.Venue;

public class GetVenueById(IPersistVenues venueRepository)
{
    public async Task<Domain.Events.Venue.Venue?> Execute(Guid id)
    {
        return await venueRepository.GetById(id);
    }
}