using Application.Tickets.Ticket.PurchaseTickets;
using Controllers.Tickets.Requests;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets.Ticket;

[ApiController]
[Authorize(Roles = Roles.Customer)]
public class PurchaseTicketsEndpoint(PurchaseTickets purchaseTickets) : ControllerBase
{
    [HttpPost(Routes.TicketsPurchase)]
    public async Task<ActionResult> PurchaseTickets([FromRoute] Guid id, [FromBody] TicketPurchasePayload payload)
    {
        var userId = User.GetUserId();
        await purchaseTickets.Execute(id, userId, payload.ticketIds);
        return NoContent();
    }
}