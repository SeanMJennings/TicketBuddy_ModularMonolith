using Domain.Tickets.Core;
using Domain.Tickets.Event;
using EventUpserted = Messages.Events.EventUpserted;

namespace Application.Tickets.Event;

public class SyncEvent(
    IPersistEvents eventRepository,
    ITicketsUnitOfWork unitOfWork)
{
    public async Task Execute(EventUpserted message)
    {
        await eventRepository.Save(Domain.Tickets.Event.Event.Create(message.Id, message.EventName,
            message.StartDate, message.EndDate, message.Venue, message.Price));
        await unitOfWork.Commit();
    }
}

