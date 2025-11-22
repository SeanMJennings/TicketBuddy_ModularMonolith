using Application.Tickets.Commands;
using Application.Tickets.Queries;
using Controllers.Tickets.Requests;
using Domain.Tickets.Queries;
using Keycloak.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets;

[ApiController]
[Authorize(Roles = Roles.Customer)]
public class TicketController(
    TicketCommands ticketCommands,
    TicketQueries ticketQueries)
    : ControllerBase
{
    [HttpGet(Routes.Tickets)]
    public async Task<IList<Ticket>> GetTickets([FromRoute] Guid id)
    {
        return await ticketQueries.GetTickets(id);
    }

    [HttpPost(Routes.TicketsPurchase)]
    public async Task<ActionResult> PurchaseTickets([FromRoute] Guid id, [FromBody] TicketPurchasePayload payload)
    {
        await ticketCommands.PurchaseTickets(id, payload.userId, payload.ticketIds);
        return NoContent();
    }

    [HttpGet(Routes.TicketsPurchased)]
    public async Task<IList<Ticket>> GetTicketsForUser([FromRoute] Guid userId)
    {
        return await ticketQueries.GetTicketsForUser(userId);
    }
    
    [HttpPost(Routes.TicketsReservation)]
    public async Task<ActionResult> ReserveTickets([FromRoute] Guid id, [FromBody] TicketReservationPayload payload)
    {
        await ticketCommands.ReserveTickets(id, payload.userId, payload.ticketIds);
        return NoContent();
    }
}