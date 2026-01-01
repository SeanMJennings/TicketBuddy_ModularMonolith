namespace Domain.Events;

public interface IPersistEvents
{
    public Task Add(Event theEvent);
    public Task Update(Event theEvent);
    public Task<Event?> Get(Guid id);
    public Task<IList<Event>> GetAll();
}