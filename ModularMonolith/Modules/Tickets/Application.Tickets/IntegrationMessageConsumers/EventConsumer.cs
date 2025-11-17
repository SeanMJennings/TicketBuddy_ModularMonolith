using Domain.Tickets.Contracts;
using MassTransit;
using Event = Domain.Tickets.Entities.Event;
using EventUpserted = Integration.Events.Messaging.EventUpserted;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class EventConsumer(IPersistEvents eventRepository) : IConsumer<EventUpserted>
    {
        public async Task Consume(ConsumeContext<EventUpserted> context)
        {
            Console.WriteLine($"Received EventUpserted message for Event ID: {context.Message.Id}");
            var theVenue = await eventRepository.GetByVenueId(context.Message.Venue);
            await eventRepository.Save(Event.Create(context.Message.Id,context.Message.EventName, context.Message.StartDate, context.Message.EndDate, theVenue, context.Message.Price));
            await eventRepository.Commit();
        }
    }
}