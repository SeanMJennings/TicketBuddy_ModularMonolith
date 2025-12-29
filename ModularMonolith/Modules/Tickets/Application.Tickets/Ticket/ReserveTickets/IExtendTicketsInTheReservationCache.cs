namespace Application.Tickets.Ticket.ReserveTickets;

public interface IExtendTicketsInTheReservationCache
{
    public Task ExtendTicketReservationForUser(Guid eventId, Guid ticketId, Guid userId);
}