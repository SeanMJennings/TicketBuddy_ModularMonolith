namespace Domain.Tickets.Ticket;

public interface IPersistTickets
{
    Task<IReadOnlyList<Ticket>> GetByIds(Guid[] ids);
    Task<IReadOnlyList<Ticket>> GetByEventId(Guid eventId);
    Task<int> GetAvailableCountByEventId(Guid eventId);
    Task<int> GetTotalCountByEventId(Guid eventId);
    Task AddRange(IEnumerable<Ticket> tickets);
    Task UpdateRange(IEnumerable<Ticket> tickets);
}

