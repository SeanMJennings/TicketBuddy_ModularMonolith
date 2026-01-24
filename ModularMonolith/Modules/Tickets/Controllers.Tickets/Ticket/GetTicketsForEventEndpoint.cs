﻿using Application.Tickets.Ticket.GetTicketsForEvent;
using Domain.Tickets.Ticket;
using Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Tickets.Ticket;

[ApiController]
[Authorize(Roles = UserRoles.Customer)]
public class GetTicketsForEventEndpoint(GetTicketsForEvent getTicketsForEvent) : ControllerBase
{
    [HttpGet(Routes.Tickets)]
    public async Task<IList<TicketQuery>> GetTickets([FromRoute] Guid id)
    {
        return await getTicketsForEvent.Execute(id);
    }
}