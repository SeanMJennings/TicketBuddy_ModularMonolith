using System.ComponentModel.DataAnnotations;

namespace Domain.Tickets.Ticket;

public static class TicketsPurchaser
{
    public static async Task<bool> PurchaseTickets(Guid eventId, Guid userId, Guid[] ticketIds, IPersistTickets ticketRepository)
    {
        var availableCount = await ticketRepository.GetAvailableCountByEventId(eventId);
        var tickets = await ticketRepository.GetByIds(ticketIds);
        
        if (tickets.Count != ticketIds.Length) throw new ValidationException("One or more tickets do not exist");
        if (tickets.Any(t => t.EventId != eventId)) throw new ValidationException("One or more tickets do not belong to this event");

        foreach (var ticket in tickets) ticket.Purchase(userId);

        await ticketRepository.UpdateRange(tickets);
        
        return availableCount - tickets.Count == 0;
    }
}

