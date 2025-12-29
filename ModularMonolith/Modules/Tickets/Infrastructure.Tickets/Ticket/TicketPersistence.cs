using Application.Tickets.Ticket;
using Domain.Tickets.Ticket;
using Infrastructure.Queries;
using Infrastructure.Tickets.Core;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Tickets.Ticket;

public class TicketRepository(TicketDbContext ticketDbContext) : IPersistTickets
{
    public async Task<IReadOnlyList<Domain.Tickets.Ticket.Ticket>> GetByIds(Guid[] ids)
    {
        return await ticketDbContext.Tickets
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Domain.Tickets.Ticket.Ticket>> GetByEventId(Guid eventId)
    {
        return await ticketDbContext.Tickets
            .Where(t => t.EventId == eventId)
            .ToListAsync();
    }

    public async Task<int> GetAvailableCountByEventId(Guid eventId)
    {
        return await ticketDbContext.Tickets
            .Where(t => t.EventId == eventId && t.UserId == null)
            .CountAsync();
    }

    public async Task<int> GetTotalCountByEventId(Guid eventId)
    {
        return await ticketDbContext.Tickets
            .Where(t => t.EventId == eventId)
            .CountAsync();
    }

    public async Task AddRange(IEnumerable<Domain.Tickets.Ticket.Ticket> tickets)
    {
        await ticketDbContext.Tickets.AddRangeAsync(tickets);
    }

    public Task UpdateRange(IEnumerable<Domain.Tickets.Ticket.Ticket> tickets)
    {
        foreach (var ticket in tickets)
        {
            ticketDbContext.Entry(ticket).State = EntityState.Modified;
        }
        return Task.CompletedTask;
    }
}

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

public class TicketReservationCacheRepository(IConnectionMultiplexer connectionMultiplexer) 
    : IPersistTicketReservationCache, IQueryTicketReservationStatus
{
    private static string GetReservationKey(Guid eventId, Guid ticketId) => $"event:{eventId}:ticket:{ticketId}:reservation";

    public async Task<Dictionary<Guid, bool>> GetTicketsReservationStatusForEvent(Guid eventId, IList<Guid> ticketIds)
    {
        var db = connectionMultiplexer.GetDatabase();
        var ticketReservationStatuses = new Dictionary<Guid, bool>();
        foreach (var ticketId in ticketIds)
        {
            var value = await db.StringGetAsync(GetReservationKey(eventId, ticketId));
            if (value.HasValue)
            {
                ticketReservationStatuses[ticketId] = true;
            }
        }
        return ticketReservationStatuses;
    }

    public async Task ExtendTicketReservationForUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var db = connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(GetReservationKey(eventId, ticketId));
        if (value.HasValue && value == userId.ToString())
        {
            await db.KeyExpireAsync(GetReservationKey(eventId, ticketId), TimeSpan.FromMinutes(15));
        }
        else
        {
            await db.StringSetAsync(GetReservationKey(eventId, ticketId), userId.ToString(), TimeSpan.FromMinutes(15));
        }
    }

    public async Task<string?> GetUserIdForTicketReservation(Guid eventId, Guid ticketId)
    {
        var db = connectionMultiplexer.GetDatabase();
        return await db.StringGetAsync(GetReservationKey(eventId, ticketId));
    }
}

