using Domain.Tickets.Contracts;
using StackExchange.Redis;

namespace Application.Tickets.Queries;

public class TicketQueries(
    IQueryTickets TicketQuerist,
    IConnectionMultiplexer ConnectionMultiplexer)
{
    public async Task<IList<Domain.Tickets.Queries.Ticket>> GetTickets(Guid eventId)
    {
        var tickets = await TicketQuerist.GetTicketsForEvent(eventId);
        await MarkTicketsWithReservationStatus(eventId, tickets);
        return tickets;
    }
    
    public async Task<IList<Domain.Tickets.Queries.Ticket>> GetTicketsForUser(Guid eventId, Guid userId)
    {
        return await TicketQuerist.GetTicketsForEventByUser(eventId, userId);
    }
    
    private static string GetReservationKey(Guid eventId, Guid ticketId) => $"event:{eventId}:ticket:{ticketId}:reservation";
    
    private async Task MarkTicketsWithReservationStatus(Guid id, IList<Domain.Tickets.Queries.Ticket> tickets)
    {
        var db = ConnectionMultiplexer.GetDatabase();
        foreach (var ticket in tickets)
        {
            var value = await db.StringGetAsync(GetReservationKey(id, ticket.Id));
            if (value.HasValue) ticket.MarkTicketAsReserved();
        }
    }
}