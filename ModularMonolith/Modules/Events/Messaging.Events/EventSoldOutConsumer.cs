using Application.Events.MessageHandlers;
using MassTransit;
using Messages.Tickets;

namespace Messaging.Events
{
    public class EventSoldOutConsumer(EventSoldOutHandler handler) : IConsumer<EventSoldOut>
    {
        public async Task Consume(ConsumeContext<EventSoldOut> context)
        {
            await handler.Handle(context.Message);
        }
    }
}