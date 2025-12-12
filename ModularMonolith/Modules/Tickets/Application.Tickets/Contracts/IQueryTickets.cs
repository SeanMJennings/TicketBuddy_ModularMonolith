using Domain.Tickets.Queries;

namespace Application.Tickets.Contracts;

public interface IQueryTickets
{
    public Task<IList<Ticket>> GetTicketsForEvent(Guid eventId);
    public Task<IList<Ticket>> GetTicketsForUser(Guid userId);
}