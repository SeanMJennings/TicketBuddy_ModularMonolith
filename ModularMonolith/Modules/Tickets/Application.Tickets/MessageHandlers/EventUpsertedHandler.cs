using Domain.Tickets.Core;
using Domain.Tickets.Event;
using EventUpserted = Messages.Events.EventUpserted;

namespace Application.Tickets.MessageHandlers
{
    public class EventUpsertedHandler(
        IPersistEvents eventRepository,
        ITicketsUnitOfWork unitOfWork)
    {
        public async Task Handle(EventUpserted message)
        {
            await eventRepository.Save(Domain.Tickets.Event.Event.Create(message.Id, message.EventName,
                message.StartDate, message.EndDate, message.Venue, message.Price));
            await unitOfWork.Commit();
        }
    }
}