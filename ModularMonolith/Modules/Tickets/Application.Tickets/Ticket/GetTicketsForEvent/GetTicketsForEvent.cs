using Domain.Tickets.Event;
using Domain.Tickets.Ticket;

namespace Application.Tickets.Ticket.GetTicketsForEvent;

public class GetTicketsForEvent(
    IQueryTickets ticketQuerist,
    IQueryTicketReservations ticketReservationCache,
    IPersistEvents eventRepository)
{
    public async Task<IList<TicketQuery>> Execute(Guid eventId)
    {
        await TicketsValidator.CheckEventExists(eventId, eventRepository);
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