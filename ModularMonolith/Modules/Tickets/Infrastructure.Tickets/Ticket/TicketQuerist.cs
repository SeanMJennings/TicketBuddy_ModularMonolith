using Domain.Tickets.Ticket;
using Infrastructure.Queries;

namespace Infrastructure.Tickets.Ticket;

public class TicketQuerist(Database database) : IQueryTickets
{
    public async Task<IList<TicketQuery>> GetTicketsForEvent(Guid eventId)
    {
        return (await database.Query<TicketQuery>("""
                                                  SELECT "Id", "EventId", "Price", "SeatNumber", ("PurchasedAt" IS NOT NULL) AS "Purchased"
                                                  FROM "Ticket"."Tickets"
                                                  WHERE "EventId" = @EventId
                                                  """, new { EventId = eventId })).ToList();
    }
    
        
    public async Task<IList<TicketQuery>> GetTicketsForUser(Guid userId)
    {
        return (await database.Query<TicketQuery>("""
                                                  SELECT "Id", "EventId", "Price", "SeatNumber", ("PurchasedAt" IS NOT NULL) AS "Purchased"
                                                  FROM "Ticket"."Tickets"
                                                  WHERE "UserId" = @UserId
                                                  """, new { UserId = userId })).ToList();
    }
}