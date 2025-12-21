namespace Application.Contracts;

public interface IManageAUnitOfWork
{
    Task Commit(CancellationToken cancellationToken = default);
}