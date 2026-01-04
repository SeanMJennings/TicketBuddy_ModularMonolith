using Application.Events.Venue;
using Controllers.Events.Requests;
using Domain.Events.Venue;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events.Venue;

[ApiController]
[Authorize(Roles = Roles.Admin)]
public class UpdateVenueEndpoint(UpdateVenue updateVenue) : ControllerBase
{
    [HttpPut(Routes.TheVenue)]
    public async Task<ActionResult> UpdateVenue(Guid id, [FromBody] UpdateVenuePayload payload)
    {
        await updateVenue.Execute(id, payload.Name, new Address(payload.Street, payload.City, payload.Postcode), payload.Capacity);
        return NoContent();
    }
}
