using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tickets.Commands;

public class TicketRepository(TicketDbContext ticketDbContext) : IPersistTickets
{
    public async Task<IReadOnlyList<Ticket>> GetByIds(Guid[] ids)
    {
        return await ticketDbContext.Tickets
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Ticket>> GetByEventId(Guid eventId)
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

    public async Task<int> GetTotalCountByEventId(Guid eventId)
    {
        return await ticketDbContext.Tickets
            .Where(t => t.EventId == eventId)
            .CountAsync();
    }

    public async Task SaveRange(IEnumerable<Ticket> tickets)
    {
        await ticketDbContext.Tickets.AddRangeAsync(tickets);
    }

    public Task UpdateRange(IEnumerable<Ticket> tickets)
    {
        foreach (var ticket in tickets)
        {
            ticketDbContext.Entry(ticket).State = EntityState.Modified;
        }
        return Task.CompletedTask;
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
}

