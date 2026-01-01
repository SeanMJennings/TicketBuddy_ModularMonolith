using Domain.Events;

namespace Application.Events;

public class GetEvents(IPersistEvents eventRepository)
{
    public async Task<IList<Event>> Execute()
    {
        return await eventRepository.GetAll();
    }
}