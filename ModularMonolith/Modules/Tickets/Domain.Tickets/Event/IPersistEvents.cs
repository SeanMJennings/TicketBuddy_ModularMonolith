namespace Domain.Tickets.Event;

public interface IPersistEvents
{
    public Task<Event?> GetById(Guid id);
    public Task Upsert(Event theEvent);
}