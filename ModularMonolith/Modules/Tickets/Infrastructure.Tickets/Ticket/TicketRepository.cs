using Domain.Tickets.Ticket;
using Infrastructure.Tickets.Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tickets.Ticket;

public class TicketRepository(TicketDbContext ticketDbContext) : IPersistTickets
{
    public async Task<IReadOnlyList<Domain.Tickets.Ticket.Ticket>> GetByIds(Guid[] ids)
    {
        return await ticketDbContext.Tickets
            .Where(t => ((IEnumerable<Guid>)ids).Contains(t.Id))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Domain.Tickets.Ticket.Ticket>> GetByEventId(Guid eventId)
    {
        return await ticketDbContext.Tickets
            .Where(t => t.EventId == eventId)
            .ToListAsync();
    }

    public async Task<int> GetAvailableCountByEventId(Guid eventId)
    {
        return await ticketDbContext.Tickets
            .Where(t => t.EventId == eventId && t.UserId == null)
            .CountAsync();
    }

    public async Task AddRange(IEnumerable<Domain.Tickets.Ticket.Ticket> tickets)
    {
        await ticketDbContext.Tickets.AddRangeAsync(tickets);
    }

    public Task UpdateRange(IEnumerable<Domain.Tickets.Ticket.Ticket> tickets)
    {
        foreach (var ticket in tickets)
        {
            ticketDbContext.Entry(ticket).State = EntityState.Modified;
        }
        return Task.CompletedTask;
    }
}