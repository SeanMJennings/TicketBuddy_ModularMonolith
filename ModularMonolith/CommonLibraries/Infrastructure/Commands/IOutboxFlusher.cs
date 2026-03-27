namespace Infrastructure.Commands;

public interface IOutboxFlusher
{
    Task FlushAsync(CancellationToken cancellationToken = default);
}