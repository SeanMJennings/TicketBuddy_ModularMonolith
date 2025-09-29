using Application.Events;
using Controllers.Events.Requests;
using Domain.Events.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events;

[ApiController]
public class EventController(EventService EventService) : ControllerBase
{
    [HttpGet(Routes.Events)]
    public async Task<IList<Event>> GetEvents()
    {
        return await EventService.GetEvents();
    }    
    
    [HttpGet(Routes.TheEvent)]
    public async Task<ActionResult<Event>> GetEvent(Guid id)
    {
        var @event = await EventService.GetEventById(id);
        if (@event is null) return NotFound();
        return @event;
    }    
    
    [HttpPost(Routes.Events)]
    public async Task<ActionResult<Guid>> CreateEvent([FromBody] EventPayload payload)
    {
        var eventId = await EventService.CreateEvent(payload.EventName, payload.StartDate, payload.EndDate, payload.Price);
        return Created($"/{Routes.Events}/{eventId}", eventId);
    }

    [HttpPut(Routes.TheEvent)]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventPayload payload)
    {
        await EventService.UpdateEvent(id, payload.EventName, payload.StartDate, payload.EndDate, payload.Price);
        return NoContent();
    }
}