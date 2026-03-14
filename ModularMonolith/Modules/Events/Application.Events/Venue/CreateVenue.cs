using Domain.Events;
using Domain.Events.Venue;

namespace Application.Events.Venue;

public class CreateVenue(VenuesValidator venuesValidator, IPersistVenues venueRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task<Guid> Execute(VenueName name, Address address, uint capacity)
    {
        var venueId = Guid.CreateVersion7();
        var venue = new Domain.Events.Venue.Venue(venueId, name, address, capacity);

        await venuesValidator.CheckAddressUniqueness(address);
        await venueRepository.Add(venue);
        await unitOfWork.Commit();
        return venueId;
    }
}