using Application.Events;
using Controllers.Events.Requests;
using Domain.ValueObjects;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Events.Event;

[ApiController]
[Authorize(Roles = Roles.Admin)]
public class UpdateEventEndpoint(UpdateEvent updateEvent) : ControllerBase
{
    [HttpPut(Routes.TheEvent)]
    public async Task<ActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventPayload payload)
    {
        await updateEvent.Execute(id, payload.EventName, payload.StartDate, payload.EndDate, new Money(payload.Price));
        return NoContent();
    }
}