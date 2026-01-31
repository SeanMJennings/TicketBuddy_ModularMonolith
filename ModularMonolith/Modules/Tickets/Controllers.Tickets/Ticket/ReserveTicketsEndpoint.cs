using Application.Tickets.Ticket.ReserveTickets;
using Controllers.Tickets.Requests;
using Application;
using Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets.Ticket;

[ApiController]
[Authorize(Roles = UserRoles.Customer)]
public class ReserveTicketsEndpoint(ReserveTickets reserveTickets) : ControllerBase
{
    [HttpPost(Routes.TicketsReservation)]
    public async Task<ActionResult> FastReserveTicketsWithoutCheckingForExistence(
        [FromRoute] Guid id, [FromBody] TicketReservationPayload payload)
    {
        var userId = User.GetUserId();
        await reserveTickets.Execute(id, userId, payload.ticketIds);
        return NoContent();
    }
}