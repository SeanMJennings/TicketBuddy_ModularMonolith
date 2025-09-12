using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using StackExchange.Redis;

namespace Application.Tickets;

public class TicketService(
    IAmATicketRepositoryForCommands CommandTicketRepository,
    IAmATicketRepositoryForQueries QueryTicketRepository,
    IConnectionMultiplexer connectionMultiplexer)
{
    public async Task<IList<Domain.Tickets.Queries.Ticket>> GetTickets(Guid eventId)
    {
        var tickets = await QueryTicketRepository.GetTicketsForEvent(eventId);
        await MarkTicketsWithReservationStatus(eventId, tickets);
        return tickets;
    }

    public async Task PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
        }

        var tickets = await CommandTicketRepository.GetTicketsForEvent(eventId);
        var theTickets = tickets as Ticket[] ?? tickets.ToArray();
        theTickets = theTickets.Where(t => ticketIds.Contains(t.Id)).ToArray();
        if (theTickets.Length != ticketIds.Length) throw new ValidationException("One or more tickets do not exist");
        
        foreach (var ticket in theTickets)
        {
            ticket.Purchase(userId);
        }
        CommandTicketRepository.UpdateTickets(theTickets);
        await CommandTicketRepository.Commit();
    }

    public async Task<IList<Domain.Tickets.Queries.Ticket>> GetTicketsForUser(Guid eventId, Guid userId)
    {
        return await QueryTicketRepository.GetTicketsForEventByUser(eventId, userId);
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