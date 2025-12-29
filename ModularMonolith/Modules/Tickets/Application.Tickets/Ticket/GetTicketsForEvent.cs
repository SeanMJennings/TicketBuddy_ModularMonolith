using Domain.Tickets.Ticket;

namespace Application.Tickets.Ticket;

public interface IQueryTickets
{
    public Task<IList<TicketQuery>> GetTicketsForEvent(Guid eventId);
    public Task<IList<TicketQuery>> GetTicketsForUser(Guid userId);
}

public interface IQueryTicketReservationStatus
{
    public Task<Dictionary<Guid, bool>> GetTicketsReservationStatusForEvent(Guid eventId, IList<Guid> ticketIds);
}

public class GetTicketsForEvent(
    IQueryTickets ticketQuerist,
    IQueryTicketReservationStatus ticketReservationCache)
{
    public async Task<IList<TicketQuery>> Execute(Guid eventId)
    {
        var tickets = await ticketQuerist.GetTicketsForEvent(eventId);
        await MarkTicketsWithReservationStatus(eventId, tickets);
        return tickets;
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

