using Domain.Users.Contracts;
using Domain.Users.Entities;

namespace Application.Users.Queries;

public class UserQueries(IAmAUserRepository UserRepository)
{
    public async Task<IList<User>> GetUsers()
    {
        return await UserRepository.GetAll();
    }

    public async Task<User?> Get(Guid id)
    {
        return await UserRepository.Get(id);
    }
}