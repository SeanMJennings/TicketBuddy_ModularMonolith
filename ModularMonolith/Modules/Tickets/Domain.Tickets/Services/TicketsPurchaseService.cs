using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;

namespace Domain.Tickets.Services;

public static class TicketsPurchaseService
{
    public static async Task<bool> PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds, IPersistTickets ticketRepository)
    {
        var tickets = await ticketRepository.GetByIds(ticketIds);
        
        if (tickets.Count != ticketIds.Length) throw new ValidationException("One or more tickets do not exist");

        if (tickets.Any(t => t.EventId != eventId)) throw new ValidationException("One or more tickets do not belong to this event");

        foreach (var ticket in tickets) ticket.Purchase(userId);

        await ticketRepository.UpdateRange(tickets);
        await ticketRepository.Commit();
        
        return await ticketRepository.GetAvailableCountByEventId(eventId) == 0;
    }
}

