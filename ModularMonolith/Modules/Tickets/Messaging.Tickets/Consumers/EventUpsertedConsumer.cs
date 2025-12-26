using Application.Tickets.MessageHandlers;
using MassTransit;
using EventUpserted = Messages.Events.EventUpserted;

namespace Messaging.Tickets.Consumers
{
    public class EventUpsertedConsumer(EventUpsertedHandler handler) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            await handler.Handle(context.Message);
        }
    }
}