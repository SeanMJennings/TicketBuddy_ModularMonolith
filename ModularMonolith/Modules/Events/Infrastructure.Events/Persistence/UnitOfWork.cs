using Domain.Contracts;

namespace Infrastructure.Events.Persistence;

public class UnitOfWork(EventDbContext eventDbContext) : IUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await eventDbContext.Commit(cancellationToken);
    }
}