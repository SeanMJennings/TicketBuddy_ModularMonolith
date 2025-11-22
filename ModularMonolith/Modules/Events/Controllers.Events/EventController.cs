using Application.Events.Commands;
using Application.Events.Queries;
using Controllers.Events.Requests;
using Domain.Events.Entities;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events;

[ApiController]
public class EventController(EventCommands eventCommands, EventQueries eventQueries) : ControllerBase
{
    [HttpGet(Routes.Events)]
    public async Task<IList<Event>> GetEvents()
    {
        return await eventQueries.GetEvents();
    }    
    
    [HttpGet(Routes.TheEvent)]
    public async Task<ActionResult<Event>> GetEvent(Guid id)
    {
        var @event = await eventQueries.GetEventById(id);
        if (@event is null) return NotFound();
        return @event;
    }    
    
    [Authorize(Roles = Roles.Admin)]
    [HttpPost(Routes.Events)]
    public async Task<CreatedResult> CreateEvent([FromBody] EventPayload payload)
    {
        var eventId = await eventCommands.CreateEvent(payload.EventName, payload.StartDate, payload.EndDate, payload.Price);
        return Created($"/{Routes.Events}/{eventId}", eventId);
    }

    [HttpPut(Routes.TheEvent)]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventPayload payload)
    {
        await eventCommands.UpdateEvent(id, payload.EventName, payload.StartDate, payload.EndDate, payload.Price);
        return NoContent();
    }
}