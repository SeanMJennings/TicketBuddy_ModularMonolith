using Domain.Events;
using Messages.Tickets;

namespace Application.Events;

public class MarkEventAsSoldOut(IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
{
    public async Task Execute(EventSoldOut message)
    {
        var theEvent = await eventRepository.Get(message.EventId);
        if (theEvent is null) return;
        
        theEvent.MarkAsSoldOut();
        await eventRepository.Update(theEvent);
        await unitOfWork.Commit();
    }
}