using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Event;

namespace Domain.Tickets.Ticket;

public static class TicketsValidator
{
    public static async Task<Event.Event> CheckEventExists(Guid eventId, IPersistEvents eventRepository)
    {
        var existingEvent = await eventRepository.GetById(eventId);
        return existingEvent ?? throw new ValidationException($"Event with id {eventId} not found");
    }

    public static async Task<IReadOnlyList<Ticket>> CheckTicketsExist(Guid[] ticketIds, IPersistTickets ticketRepository)
    {
        var tickets = await ticketRepository.GetByIds(ticketIds);
        
        return tickets.Count != ticketIds.Length ? throw new ValidationException("One or more tickets do not exist") : tickets;
    }
    
    public static void CheckIfTicketReservedForDifferentUser(Guid userId, string? reservedUserId)
    {
        if (reservedUserId != null && reservedUserId != userId.ToString())
        {
            throw new ValidationException("Ticket already reserved");
        }
    }
    
    public static void EnsureTicketReservedForUser(Guid userId, string? reservedUserId)
    {
        if (reservedUserId != userId.ToString())
        {
            throw new ValidationException("Ticket not reserved for this user");
        }
    }
}