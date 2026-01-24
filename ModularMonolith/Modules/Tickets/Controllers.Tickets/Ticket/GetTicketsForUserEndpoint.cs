using Application.Tickets.Ticket.GetTicketsForUser;
using Domain.Tickets.Ticket;
using Application;
using Application.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets.Ticket;

[ApiController]
[Authorize(Roles = UserRoles.Customer)]
public class GetTicketsForUserEndpoint(GetTicketsForUser getTicketsForUser) : ControllerBase
{
    [HttpGet(Routes.TicketsPurchased)]
    public async Task<IList<TicketQuery>> GetTicketsForUser()
    {
        var userId = User.GetUserId();
        return await getTicketsForUser.Execute(userId);
    }
}