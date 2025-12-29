using Application.Events;
using MassTransit;
using Messages.Tickets;

namespace Messaging.Events
{
    public class EventSoldOutConsumer(MarkEventAsSoldOut markEventAsSoldOut) : IConsumer<EventSoldOut>
    {
        public async Task Consume(ConsumeContext<EventSoldOut> context)
        {
            await markEventAsSoldOut.Execute(context.Message);
        }
    }
}