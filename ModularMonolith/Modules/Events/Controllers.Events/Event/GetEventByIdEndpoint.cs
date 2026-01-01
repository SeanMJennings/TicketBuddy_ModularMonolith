using Application.Events;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events;

[ApiController]
public class GetEventByIdEndpoint(GetEventById getEventById) : ControllerBase
{
    [HttpGet(Routes.TheEvent)]
    public async Task<ActionResult<Domain.Events.Event>> GetEvent(Guid id)
    {
        var @event = await getEventById.Execute(id);
        if (@event is null) return NotFound();
        return @event;
    }
}