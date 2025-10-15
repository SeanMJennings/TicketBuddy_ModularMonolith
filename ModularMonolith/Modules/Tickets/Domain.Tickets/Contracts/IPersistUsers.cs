using Domain.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IPersistUsers : IPersist
{
    public Task Save(User user);
}