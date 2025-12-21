using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IPersistEvents
{
    public Task<Event?> GetById(Guid id);
    public Task<Venue> GetByVenueId(Domain.ValueObjects.Venue venue);
    public Task Save(Event theEvent);
}