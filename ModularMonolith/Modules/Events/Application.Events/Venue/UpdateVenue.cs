using Domain.Events;
using Domain.Events.Venue;

namespace Application.Events.Venue;

public class UpdateVenue(IPersistVenues venueRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task Execute(Guid venueId, VenueName venueName, Address address, uint capacity)
    {
        var existingVenue = VenuesValidator.CheckVenueExists(await venueRepository.GetById(venueId), venueId);
        existingVenue.UpdateName(venueName);
        existingVenue.UpdateAddress(address);
        existingVenue.UpdateCapacity(capacity);

        var allVenues = await venueRepository.GetAll();
        VenuesValidator.CheckAddressUniqueness(address, allVenues, venueId);
        await venueRepository.Update(existingVenue);
        await unitOfWork.Commit();
    }
}