namespace Infrastructure.Commands;

public class NoOpOutboxFlusher : IOutboxFlusher
{
    public Task FlushAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}