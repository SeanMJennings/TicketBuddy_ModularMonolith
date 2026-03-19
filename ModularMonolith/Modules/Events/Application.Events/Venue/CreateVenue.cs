using Domain.Events;
using Domain.Events.Venue;

namespace Application.Events.Venue;

public class CreateVenue(IPersistVenues venueRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task<Guid> Execute(VenueName name, Address address, uint capacity)
    {
        var venueId = Guid.CreateVersion7();
        var venue = new Domain.Events.Venue.Venue(venueId, name, address, capacity);

        var allVenues = await venueRepository.GetAll();
        VenuesValidator.CheckAddressUniqueness(address, allVenues);
        await venueRepository.Add(venue);
        await unitOfWork.Commit();
        return venueId;
    }
}