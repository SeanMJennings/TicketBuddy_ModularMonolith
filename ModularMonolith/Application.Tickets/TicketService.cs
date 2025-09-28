using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using StackExchange.Redis;

namespace Application.Tickets;

public class TicketService(
    IAmAnEventRepository eventRepository,
    IQueryTickets TicketQuerier,
    IConnectionMultiplexer connectionMultiplexer)
{
    public async Task<IList<Domain.Tickets.Queries.Ticket>> GetTickets(Guid eventId)
    {
        var tickets = await TicketQuerier.GetTicketsForEvent(eventId);
        await MarkTicketsWithReservationStatus(eventId, tickets);
        return tickets;
    }

    public async Task PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
        }
        var theEvent = await eventRepository.GetById(eventId);
        if (theEvent is null) throw new ValidationException("Event does not exist");
        
        theEvent.PurchaseTickets(userId, ticketIds);
        await eventRepository.Save(theEvent);
        await eventRepository.Commit();
    }

    public async Task<IList<Domain.Tickets.Queries.Ticket>> GetTicketsForUser(Guid eventId, Guid userId)
    {
        return await TicketQuerier.GetTicketsForEventByUser(eventId, userId);
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
    
    private async Task MarkTicketsWithReservationStatus(Guid id, IList<Domain.Tickets.Queries.Ticket> tickets)
    {
        var db = connectionMultiplexer.GetDatabase();
        foreach (var ticket in tickets)
        {
            var value = await db.StringGetAsync(GetReservationKey(id, ticket.Id));
            if (value.HasValue) ticket.MarkTicketAsReserved();
        }
    }
    
    private async Task CheckIfTicketReservedForDifferentUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var db = connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(GetReservationKey(eventId, ticketId));
        if (value.HasValue && value != userId.ToString()) throw new ValidationException("Tickets already reserved");
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