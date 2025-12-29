using Domain.Tickets.Core;

namespace Infrastructure.Tickets.Commands;

public class UnitOfWork(TicketDbContext ticketDbContext) : ITicketsUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
}