using Domain.Events;

namespace Application.Events;

public class GetEventById(IPersistEvents eventRepository)
{
    public async Task<Event?> Execute(Guid eventId)
    {
        return await eventRepository.Get(eventId);
    }
}