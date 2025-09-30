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
            var theVenue = await eventRepository.GetByVenueId(context.Message.Venue);
            await eventRepository.Save(Event.Create(context.Message.Id,context.Message.EventName, context.Message.StartDate, context.Message.EndDate, theVenue, context.Message.Price));
            await eventRepository.Commit();
        }
    }
    
    public class EventConsumerDefinition : ConsumerDefinition<EventConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EventConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}