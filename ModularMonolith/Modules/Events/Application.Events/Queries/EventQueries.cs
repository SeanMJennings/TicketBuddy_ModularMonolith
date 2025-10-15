using Domain.Events.Contracts;
using Domain.Events.Entities;

namespace Application.Events.Queries;

public class EventQueries(IPersistEvents EventRepository)
{
    public async Task<IList<Event>> GetEvents()
    {
        return await EventRepository.GetAll();
    }
    
    public async Task<Event?> GetEventById(Guid eventId)
    {
        return await EventRepository.Get(eventId);
    }
}