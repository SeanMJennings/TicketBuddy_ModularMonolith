namespace Domain.Tickets.Event;

public interface IPersistEvents
{
    public Task<Event?> GetById(Guid id);
    public Task<Venue.Venue> GetByVenueId(Domain.ValueObjects.Venue venue);
    public Task Save(Event theEvent);
}