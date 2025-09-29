using Domain.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IAmAUserRepository : IAmARepository
{
    public Task Save(User user);
}