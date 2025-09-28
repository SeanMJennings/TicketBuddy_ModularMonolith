using Domain.Contracts;
using Domain.Events.Entities;

namespace Domain.Events.Contracts;

public interface IAmAnEventRepository : IAmARepository
{
    public Task Add(Event theEvent);
    public Task Update(Event theEvent);
    public Task<Event?> Get(Guid id);
    public Task<IList<Event>> GetAll();
}