namespace Domain.Contracts;

public interface IAmAUnitOfWork
{
    Task<int> Commit(CancellationToken cancellationToken = default);
}