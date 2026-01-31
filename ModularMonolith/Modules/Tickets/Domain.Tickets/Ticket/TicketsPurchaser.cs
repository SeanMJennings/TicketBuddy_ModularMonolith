using System.ComponentModel.DataAnnotations;

namespace Domain.Tickets.Ticket;

public static class TicketsPurchaser
{
    public static async Task<bool> PurchaseTickets(Guid eventId, Guid userId, IReadOnlyList<Ticket> tickets, IPersistTickets ticketRepository)
    {
        var availableCount = await ticketRepository.GetAvailableCountByEventId(eventId);
        
        if (tickets.Any(t => t.EventId != eventId)) throw new ValidationException("One or more tickets do not belong to this event");

        foreach (var ticket in tickets) ticket.Purchase(userId);

        await ticketRepository.UpdateRange(tickets);
        
        return availableCount - tickets.Count == 0;
    }
}