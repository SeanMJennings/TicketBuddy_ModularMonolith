using Domain.Events.Contracts;

namespace Application.Events.Event;

public class GetEventById(IPersistEvents eventRepository)
{
    public async Task<Domain.Events.Entities.Event?> Execute(Guid eventId)
    {
        return await eventRepository.Get(eventId);
    }
}

