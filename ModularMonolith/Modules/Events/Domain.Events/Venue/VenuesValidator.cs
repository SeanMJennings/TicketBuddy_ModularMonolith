using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;

namespace Domain.Events.Venue;

public static class VenuesValidator
{
    public static Venue CheckVenueExists(Venue? venue, Guid venueId) =>
        venue ?? throw new EntityNotFoundException(nameof(Venue), venueId);

    public static void CheckAddressUniqueness(Address address, IEnumerable<Venue> venues, Guid? excludeVenueId = null)
    {
        var existingVenue = venues
            .Where(v => excludeVenueId == null || v.Id != excludeVenueId.Value)
            .FirstOrDefault(v => v.Address.Equals(address));

        if (existingVenue is not null) throw new ValidationException("A venue already exists at this address");
    }
}