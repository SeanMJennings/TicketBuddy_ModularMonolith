using Domain.Tickets.Ticket;

namespace Application.Tickets.Ticket.ReserveTickets;

public class ReserveTickets(
    IQueryTicketReservations queryTicketReservationCache,
    IExtendTicketsInTheReservationCache ticketReservationCache)
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
        var userIdForReservation = await queryTicketReservationCache.GetUserIdForTicketReservation(eventId, ticketId);
        TicketsValidator.CheckIfTicketReservedForDifferentUser(userId, userIdForReservation);
    }
}