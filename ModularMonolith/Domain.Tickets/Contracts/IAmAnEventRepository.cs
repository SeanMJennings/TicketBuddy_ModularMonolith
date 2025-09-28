using Domain.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IAmAnEventRepository : IAmARepository
{
    public Task<Event?> GetById(Guid Id);
    public Task Save(Event theEvent);
}