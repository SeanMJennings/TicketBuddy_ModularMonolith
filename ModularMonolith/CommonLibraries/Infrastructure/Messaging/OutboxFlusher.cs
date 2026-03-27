using Infrastructure.Commands;

namespace Infrastructure.Messaging;

public class OutboxFlusher(OutboxDbContext outboxDbContext) : IOutboxFlusher
{
    public Task FlushAsync(CancellationToken cancellationToken = default)
        => outboxDbContext.SaveChangesAsync(cancellationToken);
}