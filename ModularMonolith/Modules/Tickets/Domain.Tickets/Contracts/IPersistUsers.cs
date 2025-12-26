using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IPersistUsers
{
    public Task Save(User user);
}