using Application.Events.Venue;
using Controllers.Events.Requests;
using Domain.Events.Venue;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events.Venue;

[ApiController]
[Authorize(Roles = Roles.Admin)]
public class CreateVenueEndpoint(CreateVenue createVenue) : ControllerBase
{
    [HttpPost(Routes.Venues)]
    public async Task<CreatedResult> CreateVenue([FromBody] VenuePayload payload)
    {
        var address = new Address(payload.Street, payload.City, payload.Postcode);
        var venueId = await createVenue.Execute(payload.Name, address, payload.Capacity);
        return Created($"/{Routes.Venues}/{venueId}", venueId);
    }
}