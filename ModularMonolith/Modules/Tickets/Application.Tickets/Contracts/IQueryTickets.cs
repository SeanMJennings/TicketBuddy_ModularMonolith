using Domain.Tickets.Ticket;

namespace Application.Tickets.Contracts;

public interface IQueryTickets
{
    public Task<IList<TicketQuery>> GetTicketsForEvent(Guid eventId);
    public Task<IList<TicketQuery>> GetTicketsForUser(Guid userId);
}