using System.ComponentModel.DataAnnotations;

namespace Domain.Events.Venue;

public class VenuesValidator(IPersistVenues venueRepository)
{
    public async Task CheckAddressUniqueness(Address address)
    {
        var venues = await venueRepository.GetAll();
        var existingVenue = venues.FirstOrDefault(v => v.Address.Equals(address));
        
        if (existingVenue is not null)
        {
            throw new ValidationException("A venue already exists at this address");
        }
    }
}
