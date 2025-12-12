using Domain.Events.Contracts;
using Domain.Events.Entities;

namespace Application.Events.Queries;

public class EventQueries(IPersistEvents eventRepository)
{
    public async Task<IList<Event>> GetEvents()
    {
        return await eventRepository.GetAll();
    }
    
    public async Task<Event?> GetEventById(Guid eventId)
    {
        return await eventRepository.Get(eventId);
    }
}