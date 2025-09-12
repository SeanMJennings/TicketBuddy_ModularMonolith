using Domain.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IAmATicketRepositoryForCommands : IAmACommandRepository
{
    public void AddTickets(IEnumerable<Ticket> tickets);
    public void UpdateTickets(IEnumerable<Ticket> tickets);
    public Task<IEnumerable<Ticket>> GetTicketsForEvent(Guid eventId);
}