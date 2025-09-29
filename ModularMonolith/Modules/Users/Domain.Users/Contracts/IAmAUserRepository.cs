using Domain.Contracts;
using Domain.Users.Entities;

namespace Domain.Users.Contracts;

public interface IAmAUserRepository : IAmARepository
{
    public Task Add(User theUser);
    public Task Update(User theUser);
    public Task<User?> Get(Guid id);
    public Task<IList<User>> GetAll();
}