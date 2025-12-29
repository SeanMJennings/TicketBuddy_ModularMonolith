using Domain.Tickets.Ticket;

namespace Application.Tickets.Ticket.GetTicketsForUser;

public class GetTicketsForUser(IQueryTickets ticketQuerist)
{
    public async Task<IList<TicketQuery>> Execute(Guid userId)
    {
        return await ticketQuerist.GetTicketsForUser(userId);
    }
}