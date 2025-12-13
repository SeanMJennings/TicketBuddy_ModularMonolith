namespace Application.Contracts;

public interface IManageAUnitOfWork
{
    Task<int> Commit(CancellationToken cancellationToken = default);
}