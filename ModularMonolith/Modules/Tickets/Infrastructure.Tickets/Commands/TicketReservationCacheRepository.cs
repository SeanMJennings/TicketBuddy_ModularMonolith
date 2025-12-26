using Application.Tickets.Contracts;
using StackExchange.Redis;

namespace Infrastructure.Tickets.Commands;

public class TicketReservationCacheRepository(IConnectionMultiplexer connectionMultiplexer) : IPersistTicketReservationCache
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