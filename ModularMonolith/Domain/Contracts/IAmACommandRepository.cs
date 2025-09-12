namespace Domain.Contracts;

public interface IAmACommandRepository
{
    public Task Commit(CancellationToken cancellationToken = default);
}