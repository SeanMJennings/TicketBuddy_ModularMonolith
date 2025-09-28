namespace Domain.Contracts;

public interface IAmARepository
{
    public Task Commit(CancellationToken cancellationToken = default);
}