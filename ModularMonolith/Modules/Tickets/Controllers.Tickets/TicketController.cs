using Application.Tickets;
using Controllers.Tickets.Requests;
using Domain.Tickets.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets;

[ApiController]
public class TicketController(TicketService ticketService)
    : ControllerBase
{
    [HttpGet(Routes.Tickets)]
    public async Task<IList<Ticket>> GetTickets([FromRoute] Guid id)
    {
        return await ticketService.GetTickets(id);
    }

    [HttpPost(Routes.TicketsPurchase)]
    public async Task<ActionResult> PurchaseTickets([FromRoute] Guid id, [FromBody] TicketPurchasePayload payload)
    {
        await ticketService.PurchaseTickets(id, payload.userId, payload.ticketIds);
        return NoContent();
    }

    [HttpGet(Routes.TicketsPurchased)]
    public async Task<IList<Ticket>> GetTicketsForUser([FromRoute] Guid id, [FromRoute] Guid userId)
    {
        return await ticketService.GetTicketsForUser(id, userId);
    }
    
    [HttpPost(Routes.TicketsReservation)]
    public async Task<ActionResult> ReserveTickets([FromRoute] Guid id, [FromBody] TicketReservationPayload payload)
    {
        await ticketService.ReserveTickets(id, payload.userId, payload.ticketIds);
        return NoContent();
    }
}