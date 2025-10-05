using Domain.Tickets.Queries;

namespace Domain.Tickets.Contracts;

public interface IQueryTickets
{
    public Task<IList<Ticket>> GetTicketsForEvent(Guid eventId);
    public Task<IList<Ticket>> GetTicketsForUser(Guid userId);
}