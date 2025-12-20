using System.ComponentModel.DataAnnotations;
using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Services;

public static class TicketsValidator
{
    public static async Task<Event> CheckEventExists(Guid eventId, IPersistEvents eventRepository)
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