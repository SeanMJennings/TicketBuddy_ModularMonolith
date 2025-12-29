using Application.Tickets.Event;
using MassTransit;
using EventUpserted = Messages.Events.EventUpserted;

namespace Messaging.Tickets.Consumers
{
    public class EventUpsertedConsumer(UpsertEvent upsertEvent) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            await upsertEvent.Execute(context.Message);
        }
    }
}