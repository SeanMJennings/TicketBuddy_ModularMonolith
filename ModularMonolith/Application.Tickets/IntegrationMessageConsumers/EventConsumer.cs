using Domain.Tickets.Contracts;
using MassTransit;
using Event = Domain.Tickets.Entities.Event;
using EventUpserted = Integration.Events.Messaging.EventUpserted;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class EventConsumer(IAmAnEventRepository eventRepository) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            await eventRepository.Save(Event.Create(context.Message.Id,context.Message.EventName, context.Message.StartDate, context.Message.EndDate, context.Message.Venue, context.Message.Price));
            await eventRepository.Commit();
        }
    }
}