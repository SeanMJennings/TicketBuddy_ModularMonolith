using System.ComponentModel.DataAnnotations;
using Domain.Exceptions;

namespace Domain.Tickets.Ticket;

public static class TicketsValidator
{
    public static Event.Event CheckEventExists(Event.Event? theEvent, Guid eventId) =>
        theEvent ?? throw new EntityNotFoundException(nameof(Event), eventId);

    public static IReadOnlyList<Ticket> CheckTicketsExist(Guid[] ticketIds, IReadOnlyList<Ticket> tickets) =>
        tickets.Count != ticketIds.Length ? throw new ValidationException("One or more tickets do not exist") : tickets;

    public static void CheckIfTicketReservedForDifferentUser(Guid userId, string? reservedUserId)
    {
        if (reservedUserId != null && reservedUserId != userId.ToString())
            throw new ValidationException("Ticket already reserved");
    }

    public static void EnsureTicketReservedForUser(Guid userId, string? reservedUserId)
    {
        if (reservedUserId != userId.ToString())
            throw new ValidationException("Ticket not reserved for this user");
    }
}