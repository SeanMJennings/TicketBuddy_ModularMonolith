using Domain.Tickets.Contracts;
using Domain.Tickets.Queries;
using Infrastructure.Queries;

namespace Infrastructure.Tickets.Queries;

public class TicketQuerier(Database database) : IQueryTickets
{
    public async Task<IList<Ticket>> GetTicketsForEvent(Guid eventId)
    {
        return (await database.Query<Ticket>("""
                                              SELECT "Id", "EventId", "Price", "SeatNumber", ("PurchasedAt" IS NOT NULL) AS "Purchased"
                                              FROM "Ticket"."Tickets"
                                              WHERE "EventId" = @EventId
                                              """, new { EventId = eventId })).ToList();
    }
    
        
    public async Task<IList<Ticket>> GetTicketsForEventByUser(Guid eventId, Guid userId)
    {
        return (await database.Query<Ticket>("""
                                              SELECT "Id", "EventId", "Price", "SeatNumber", ("PurchasedAt" IS NOT NULL) AS "Purchased"
                                              FROM "Ticket"."Tickets"
                                              WHERE "EventId" = @EventId AND "UserId" = @UserId
                                              """, new { EventId = eventId, UserId = userId })).ToList();
    }
}