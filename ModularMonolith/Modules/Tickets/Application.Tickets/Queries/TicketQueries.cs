using Application.Tickets.Contracts;
using Domain.Tickets.Ticket;

namespace Application.Tickets.Queries;

public class TicketQueries(
    IQueryTickets ticketQuerist,
    IPersistTicketReservationCache ticketReservationCache)
{
    public async Task<IList<TicketQuery>> GetTickets(Guid eventId)
    {
        var tickets = await ticketQuerist.GetTicketsForEvent(eventId);
        await MarkTicketsWithReservationStatus(eventId, tickets);
        return tickets;
    }
    
    public async Task<IList<TicketQuery>> GetTicketsForUser(Guid userId)
    {
        return await ticketQuerist.GetTicketsForUser(userId);
    }
    
    private async Task MarkTicketsWithReservationStatus(Guid id, IList<TicketQuery> tickets)
    {
        var ticketReservationStatuses = await ticketReservationCache.GetTicketsReservationStatusForEvent(id, tickets.Select(t => t.Id).ToList());
        foreach (var ticket in tickets)
        {
            if (ticketReservationStatuses.TryGetValue(ticket.Id, out _))
            {
                ticket.MarkTicketAsReserved();
            }
        }
    }
}