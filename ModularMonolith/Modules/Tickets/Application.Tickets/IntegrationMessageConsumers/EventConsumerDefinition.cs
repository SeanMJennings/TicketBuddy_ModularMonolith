using MassTransit;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class EventConsumerDefinition : ConsumerDefinition<EventConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<EventConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}