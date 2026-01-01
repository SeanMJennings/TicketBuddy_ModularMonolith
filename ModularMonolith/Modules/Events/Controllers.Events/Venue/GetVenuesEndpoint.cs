using Application.Events.Venue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events.Venue;

[ApiController]
[AllowAnonymous]
public class GetVenuesEndpoint(GetVenues getVenues) : ControllerBase
{
    [HttpGet(Routes.Venues)]
    public async Task<IEnumerable<Domain.Events.Venue.Venue>> GetVenues()
    {
        return await getVenues.Execute();
    }
}