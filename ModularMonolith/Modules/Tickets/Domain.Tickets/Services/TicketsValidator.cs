using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Services;

public class TicketsValidator(IPersistEvents eventRepository)
{
    public async Task<Event> CheckEventExists(Guid eventId)
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