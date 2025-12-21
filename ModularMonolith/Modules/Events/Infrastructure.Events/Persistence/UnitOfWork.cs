using Domain.Events.Contracts;

namespace Infrastructure.Events.Persistence;

public class UnitOfWork(EventDbContext eventDbContext) : IEventsUnitOfWork
{
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await eventDbContext.Commit(cancellationToken);
    }
}