using Domain.Tickets.Contracts;
using MassTransit;
using Event = Domain.Tickets.Entities.Event;
using EventUpserted = Integration.Events.Messaging.EventUpserted;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class EventConsumer(IAmAnEventRepository EventRepository) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            await EventRepository.Save(new Event(context.Message.Id,context.Message.EventName, context.Message.StartDate, context.Message.EndDate, context.Message.Venue, context.Message.Price));
            await EventRepository.Commit();
        }
    }
}