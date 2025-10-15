namespace Domain.Contracts;

public interface IPersist
{
    public Task Commit(CancellationToken cancellationToken = default);
}