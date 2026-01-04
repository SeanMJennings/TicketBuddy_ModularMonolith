using Domain.Events;
using Domain.Events.Venue;

namespace Application.Events.Venue;

public class UpdateVenue(VenuesValidator venuesValidator, IPersistVenues venueRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task Execute(Guid venueId, VenueName venueName, Address address, uint capacity)
    {
        var existingVenue = await venuesValidator.CheckVenueExists(venueId);
        existingVenue.UpdateName(venueName);
        existingVenue.UpdateAddress(address);
        existingVenue.UpdateCapacity(capacity);

        await venuesValidator.CheckAddressUniqueness(address, venueId);
        await venueRepository.Update(existingVenue);
        await unitOfWork.Commit();
    }
}
