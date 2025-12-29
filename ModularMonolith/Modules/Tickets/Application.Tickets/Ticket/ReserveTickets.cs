using Domain.Tickets.Ticket;

namespace Application.Tickets.Ticket;

public class ReserveTickets(IPersistTicketReservationCache ticketReservationCache)
{
    public async Task Execute(Guid eventId, Guid userId, Guid[] ticketIds)
    {
        foreach (var ticketId in ticketIds)
        {
            await CheckIfTicketReservedForDifferentUser(eventId, ticketId, userId);
            await ticketReservationCache.ExtendTicketReservationForUser(eventId, ticketId, userId);
        }
    }
    
    private async Task CheckIfTicketReservedForDifferentUser(Guid eventId, Guid ticketId, Guid userId)
    {
        var userIdForReservation = await ticketReservationCache.GetUserIdForTicketReservation(eventId, ticketId);
        TicketsValidator.CheckIfTicketReservedForDifferentUser(userId, userIdForReservation);
    }
}

