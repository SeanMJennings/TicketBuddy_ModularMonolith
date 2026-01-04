using System.ComponentModel.DataAnnotations;

namespace Domain.Events.Venue;

public class VenuesValidator(IPersistVenues venueRepository)
{
    public async Task<Venue> CheckVenueExists(Guid venueId)
    {
        var existingVenue = await venueRepository.GetById(venueId);
        return existingVenue ?? throw new ValidationException($"Venue with id {venueId} not found");
    }

    public async Task CheckAddressUniqueness(Address address, Guid? excludeVenueId = null)
    {
        var venues = await venueRepository.GetAll();
        var existingVenue = venues
            .Where(v => excludeVenueId == null || v.Id != excludeVenueId)
            .FirstOrDefault(v => v.Address.Equals(address));

        if (existingVenue is not null) throw new ValidationException("A venue already exists at this address");
    }
}
