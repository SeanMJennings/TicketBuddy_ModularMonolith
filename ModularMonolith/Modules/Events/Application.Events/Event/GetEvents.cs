using Domain.Events.Contracts;

namespace Application.Events.Event;

public class GetEvents(IPersistEvents eventRepository)
{
    public async Task<IList<Domain.Events.Entities.Event>> Execute()
    {
        return await eventRepository.GetAll();
    }
}

