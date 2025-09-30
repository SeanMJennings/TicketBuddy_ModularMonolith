using Domain.Events.Contracts;
using Integration.Tickets.Messaging.Messages;
using MassTransit;

namespace Application.Events.IntegrationMessageConsumers
{
    public class EventSoldOutConsumer(IAmAnEventRepository eventRepository) : IConsumer<EventSoldOut>
    {
        public async Task Consume(ConsumeContext<EventSoldOut> context)
        {
            var theEvent = await eventRepository.Get(context.Message.EventId);
            if (theEvent is null) return;
            
            theEvent.MarkAsSoldOut();
            await eventRepository.Update(theEvent);
            await eventRepository.Commit();
        }
    }
    
    public class EventConsumerDefinition : ConsumerDefinition<EventSoldOutConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EventSoldOutConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}