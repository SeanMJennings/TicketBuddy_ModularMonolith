using Domain.Contracts;

namespace Infrastructure.Tickets.Commands;

public class UnitOfWork(TicketDbContext ticketDbContext) : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
}