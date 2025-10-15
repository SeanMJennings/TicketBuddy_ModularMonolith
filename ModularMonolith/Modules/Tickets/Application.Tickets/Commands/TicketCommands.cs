using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using StackExchange.Redis;

namespace Application.Tickets.Commands;

public class TicketCommands(
    IPersistEvents EventRepository,
    IConnectionMultiplexer ConnectionMultiplexer)
{
    public async Task PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
        }
        var theEvent = await EventRepository.GetById(eventId);
        if (theEvent is null) throw new ValidationException("Event does not exist");
        
        theEvent.PurchaseTickets(userId, ticketIds);
        await EventRepository.Save(theEvent);
        await EventRepository.Commit();
    }

    public async Task ReserveTickets(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
            await ExtendReservation(eventId, ticketId, userId);
        }
    }
    
    private static string GetReservationKey(Guid eventId, Guid ticketId) => $"event:{eventId}:ticket:{ticketId}:reservation";
    
    private async Task CheckIfTicketReservedForDifferentUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var db = ConnectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(GetReservationKey(eventId, ticketId));
        if (value.HasValue && value != userId.ToString()) throw new ValidationException("Tickets already reserved");
    }
    
    private async Task ExtendReservation(Guid eventId, Guid ticketId, Guid userId)
    {
        var db = ConnectionMultiplexer.GetDatabase();
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
}