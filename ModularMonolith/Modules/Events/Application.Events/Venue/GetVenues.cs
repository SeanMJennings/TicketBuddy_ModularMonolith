using Domain.Events.Venue;

namespace Application.Events.Venue;

public class GetVenues(IPersistVenues venueRepository)
{
    public async Task<IEnumerable<Domain.Events.Venue.Venue>> Execute()
    {
        return await venueRepository.GetAll();
    }
}