using Domain.Tickets.Entities;

namespace Application.Tickets.Contracts;

public interface IPersistUsers
{
    public Task Save(User user);
}