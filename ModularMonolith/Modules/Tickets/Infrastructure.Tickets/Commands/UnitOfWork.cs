using Domain.Tickets.Contracts;

namespace Infrastructure.Tickets.Commands;

public class UnitOfWork(TicketDbContext ticketDbContext) : ITicketsUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
}