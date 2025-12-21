using Domain.Contracts;
using Domain.Tickets.Contracts;
using MassTransit;
using Event = Domain.Tickets.Entities.Event;
using EventUpserted = Integration.Events.Messaging.EventUpserted;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class EventConsumer(
        IPersistEvents eventRepository,
        IUnitOfWork unitOfWork) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            var theVenue = await eventRepository.GetByVenueId(context.Message.Venue);
            var existingEvent = await eventRepository.GetById(context.Message.Id);
            var isNewEvent = existingEvent is null;

            if (isNewEvent)
            {
                await eventRepository.Save(Event.CreateNew(context.Message.Id, context.Message.EventName,
                    context.Message.StartDate, context.Message.EndDate, theVenue, context.Message.Price));
                await unitOfWork.Commit();
                return;
            }
            
            await eventRepository.Save(Event.CreateExisting(context.Message.Id, context.Message.EventName,
                context.Message.StartDate, context.Message.EndDate, theVenue, context.Message.Price));
            await unitOfWork.Commit();
        }
    }
}