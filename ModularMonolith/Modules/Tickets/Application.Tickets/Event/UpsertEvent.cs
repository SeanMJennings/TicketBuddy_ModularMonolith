using Domain.Tickets.Core;
using Domain.Tickets.Event;
using EventUpserted = Messages.Events.EventUpserted;

namespace Application.Tickets.Event;

public class UpsertEvent(
    IPersistEvents eventRepository,
    ITicketsUnitOfWork unitOfWork)
{
    public async Task Execute(EventUpserted message)
    {
        await eventRepository.Upsert(Domain.Tickets.Event.Event.Create(message.Id, message.EventName,
            message.StartDate, message.EndDate, message.VenueId, message.Price));
        await unitOfWork.Commit();
    }
}