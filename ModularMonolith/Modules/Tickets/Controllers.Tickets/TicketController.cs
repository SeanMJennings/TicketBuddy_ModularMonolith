using Application.Tickets.Ticket;
using Controllers.Tickets.Requests;
using Domain.Tickets.Ticket;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets;

[ApiController]
[Authorize(Roles = Roles.Customer)]
public class TicketController(
    PurchaseTickets purchaseTickets,
    ReserveTickets reserveTickets,
    GetTicketsForEvent getTicketsForEvent,
    GetTicketsForUser getTicketsForUser)
    : ControllerBase
{
    [HttpGet(Routes.Tickets)]
    public async Task<IList<TicketQuery>> GetTickets([FromRoute] Guid id)
    {
        return await getTicketsForEvent.Execute(id);
    }

    [HttpPost(Routes.TicketsPurchase)]
    public async Task<ActionResult> PurchaseTickets([FromRoute] Guid id, [FromBody] TicketPurchasePayload payload)
    {
        var userId = User.GetUserId();
        await purchaseTickets.Execute(id, userId, payload.ticketIds);
        return NoContent();
    }

    [HttpGet(Routes.TicketsPurchased)]
    public async Task<IList<TicketQuery>> GetTicketsForUser()
    {
        var userId = User.GetUserId();
        return await getTicketsForUser.Execute(userId);
    }
    
    [HttpPost(Routes.TicketsReservation)]
    public async Task<ActionResult> ReserveTickets([FromRoute] Guid id, [FromBody] TicketReservationPayload payload)
    {
        var userId = User.GetUserId();
        await reserveTickets.Execute(id, userId, payload.ticketIds);
        return NoContent();
    }
}