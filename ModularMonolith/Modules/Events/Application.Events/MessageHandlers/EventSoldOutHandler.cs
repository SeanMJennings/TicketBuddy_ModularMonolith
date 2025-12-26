using Domain.Events.Contracts;
using Messages.Tickets;

namespace Application.Events.MessageHandlers
{
    public class EventSoldOutHandler(IPersistEvents eventRepository, IEventsUnitOfWork unitOfWork)
    {
        public async Task Handle(EventSoldOut message)
        {
            var theEvent = await eventRepository.Get(message.EventId);
            if (theEvent is null) return;
            
            theEvent.MarkAsSoldOut();
            await eventRepository.Update(theEvent);
            await unitOfWork.Commit();
        }
    }
}