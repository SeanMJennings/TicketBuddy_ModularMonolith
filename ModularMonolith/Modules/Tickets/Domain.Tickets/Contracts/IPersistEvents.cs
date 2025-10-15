using Domain.Contracts;
using Domain.Tickets.Entities;

namespace Domain.Tickets.Contracts;

public interface IPersistEvents : IPersist
{
    public Task<Event?> GetById(Guid Id);
    public Task<Venue> GetByVenueId(Domain.Primitives.Venue venue);
    public Task Save(Event theEvent);
}