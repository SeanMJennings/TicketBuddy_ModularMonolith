using MassTransit;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class UserConsumerDefinition : ConsumerDefinition<UserConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UserConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}