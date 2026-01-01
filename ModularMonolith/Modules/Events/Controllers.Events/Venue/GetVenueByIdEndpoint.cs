using Application.Events.Venue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events.Venue;

[ApiController]
[AllowAnonymous]
public class GetVenueByIdEndpoint(GetVenueById getVenueById) : ControllerBase
{
    [HttpGet(Routes.TheVenue)]
    public async Task<ActionResult<Domain.Events.Venue.Venue>> GetVenue(Guid id)
    {
        var venue = await getVenueById.Execute(id);
        if (venue is null) return NotFound();
        return venue;
    }
}