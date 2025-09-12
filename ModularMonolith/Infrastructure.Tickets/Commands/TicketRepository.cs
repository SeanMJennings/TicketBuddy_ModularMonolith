using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tickets.Commands;

public class CommandTicketRepository(TicketDbContext context) : IAmATicketRepositoryForCommands
{
    public void AddTickets(IEnumerable<Ticket> tickets)
    {
        context.Tickets.AddRange(tickets);
    }

    public void UpdateTickets(IEnumerable<Ticket> tickets)
    {
        context.Tickets.UpdateRange(tickets);
    }

    public Task<IEnumerable<Ticket>> GetTicketsForEvent(Guid eventId)
    {
        return context.Tickets.Where(t => t.EventId == eventId).ToListAsync().ContinueWith(t => (IEnumerable<Ticket>)t.Result);
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await context.Commit(cancellationToken);
    }
}