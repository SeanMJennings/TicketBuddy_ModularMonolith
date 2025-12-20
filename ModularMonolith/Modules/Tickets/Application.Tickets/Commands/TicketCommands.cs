using Domain.Tickets.Contracts;
using Domain.Tickets.Services;
using StackExchange.Redis;

namespace Application.Tickets.Commands;

public class TicketCommands(
    IPersistEvents eventRepository,
    IPersistTickets ticketRepository,
    IConnectionMultiplexer connectionMultiplexer)
{
    public async Task PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
        }
        
        var theEvent = await TicketsValidator.CheckEventExists(eventId, eventRepository);
        var soldOut = await TicketsPurchaseService.PurchaseTickets(eventId, userId, ticketIds, ticketRepository);
        
        if (soldOut)
        {
            theEvent.MarkAsSoldOut();
            await eventRepository.Save(theEvent);
        }
        
        await ticketRepository.Commit();
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
        var db = connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(GetReservationKey(eventId, ticketId));
        TicketsValidator.CheckIfTicketReservedForDifferentUser(userId, value);
    }
    
    private async Task ExtendReservation(Guid eventId, Guid ticketId, Guid userId)
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
}