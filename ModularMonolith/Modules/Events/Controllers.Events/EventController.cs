using Application.Events.Event;
using Controllers.Events.Requests;
using Domain.Events.Entities;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events;

[ApiController]
public class EventController(
    CreateEvent createEvent,
    UpdateEvent updateEvent,
    GetEvents getEvents,
    GetEventById getEventById) : ControllerBase
{
    [HttpGet(Routes.Events)]
    public async Task<IList<Event>> GetEvents()
    {
        return await getEvents.Execute();
    }    
    
    [HttpGet(Routes.TheEvent)]
    public async Task<ActionResult<Event>> GetEvent(Guid id)
    {
        var @event = await getEventById.Execute(id);
        if (@event is null) return NotFound();
        return @event;
    }    
    
    [Authorize(Roles = Roles.Admin)]
    [HttpPost(Routes.Events)]
    public async Task<CreatedResult> CreateEvent([FromBody] EventPayload payload)
    {
        var eventId = await createEvent.Execute(payload.EventName, payload.StartDate, payload.EndDate, payload.Price);
        return Created($"/{Routes.Events}/{eventId}", eventId);
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut(Routes.TheEvent)]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventPayload payload)
    {
        await updateEvent.Execute(id, payload.EventName, payload.StartDate, payload.EndDate, payload.Price);
        return NoContent();
    }
}