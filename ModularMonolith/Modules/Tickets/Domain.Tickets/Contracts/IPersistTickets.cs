using Domain.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IPersistTickets : IPersist
{
    Task<IReadOnlyList<Ticket>> GetByIds(Guid[] ids);
    Task<IReadOnlyList<Ticket>> GetByEventId(Guid eventId);
    Task<int> GetAvailableCountByEventId(Guid eventId);
    Task<int> GetTotalCountByEventId(Guid eventId);
    Task SaveRange(IEnumerable<Ticket> tickets);
    Task UpdateRange(IEnumerable<Ticket> tickets);
}

