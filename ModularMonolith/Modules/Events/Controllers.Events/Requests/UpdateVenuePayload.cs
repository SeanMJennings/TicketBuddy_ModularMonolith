using Domain.Events.Venue;

namespace Controllers.Events.Requests;

public record UpdateVenuePayload(VenueName Name, string Street, string City, string Postcode, uint Capacity);
