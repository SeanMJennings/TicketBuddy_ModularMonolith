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
    
    public static void CheckIfTicketReservedForDifferentUser(Guid userId, string? reservedUserId)
    {
        if (reservedUserId != null && reservedUserId != userId.ToString())
        {
            throw new ValidationException("Tickets already reserved");
        }
    }
}