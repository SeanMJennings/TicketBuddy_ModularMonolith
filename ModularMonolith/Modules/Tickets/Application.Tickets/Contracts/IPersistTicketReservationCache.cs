namespace Application.Tickets.Contracts;

public interface IPersistTicketReservationCache
{
    public Task<string?> GetUserIdForTicketReservation(Guid eventId, Guid ticketId);
    public Task<Dictionary<Guid, bool>> GetTicketsReservationStatusForEvent(Guid eventId, IList<Guid> ticketIds);
    public Task ExtendTicketReservationForUser(Guid eventId, Guid ticketId, Guid userId);
}