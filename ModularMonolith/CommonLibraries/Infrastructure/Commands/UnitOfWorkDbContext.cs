using Infrastructure.DomainEventsDispatching;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands;

public abstract class UnitOfWorkDbContext<T>(
    DbContextOptions<T> options,
    DomainEventsDispatcher domainEventsDispatcher,
    IOutboxFlusher outboxFlusher)
    : DbContext(options)
    where T : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await domainEventsDispatcher.DispatchEvents(this);
        await SaveChangesAsync(cancellationToken);
        await outboxFlusher.FlushAsync(cancellationToken);
        ChangeTracker.Clear();
    }
}