using Application.Events;
using Application.Events;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events;

[ApiController]
public class GetEventsEndpoint(GetEvents getEvents) : ControllerBase
{
    [HttpGet(Routes.Events)]
    public async Task<IList<Domain.Events.Event>> GetEvents()
    {
        return await getEvents.Execute();
    }
}