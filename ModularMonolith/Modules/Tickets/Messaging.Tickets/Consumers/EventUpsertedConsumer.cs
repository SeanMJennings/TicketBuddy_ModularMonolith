using Application.Tickets.Event;
using MassTransit;
using EventUpserted = Messages.Events.EventUpserted;

namespace Messaging.Tickets.Consumers
{
    public class EventUpsertedConsumer(SyncEvent syncEvent) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            await syncEvent.Execute(context.Message);
        }
    }
}