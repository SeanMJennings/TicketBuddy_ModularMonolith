using Domain.Events.Contracts;
using Domain.Events.Entities;

namespace Application.Events.Queries;

public class EventQueries(IPersistEvents eventRepository)
{
    public async Task<IList<Domain.Events.Entities.Event>> GetEvents()
    {
        return await eventRepository.GetAll();
    }
    
    public async Task<Domain.Events.Entities.Event?> GetEventById(Guid eventId)
    {
        return await eventRepository.Get(eventId);
    }
}