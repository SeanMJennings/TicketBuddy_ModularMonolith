using Domain.Tickets.Queries;

namespace Domain.Tickets.Contracts;

public interface IAmATicketRepositoryForQueries
{
    public Task<IList<Ticket>> GetTicketsForEvent(Guid eventId);
    public Task<IList<Ticket>> GetTicketsForEventByUser(Guid eventId, Guid userId);
}