using Application.Tickets.MessageHandlers;
using MassTransit;
using Messaging.Keycloak.Users;

namespace Messaging.Tickets.Consumers;

public class UserRegisteredConsumer(UserRegisteredHandler handler) : IConsumer<UserRegistered>
{
    public async Task Consume(ConsumeContext<UserRegistered> context)
    {
        await handler.Handle(context.Message);
    }
}

public class UserRegisteredConsumerDefinition : ConsumerDefinition<UserRegisteredConsumer>
{
    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<UserRegisteredConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.ConfigureConsumeTopology = false;
        endpointConfigurator.ClearSerialization();
        endpointConfigurator.UseRawJsonSerializer();

        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbit) 
        {
            rabbit.Bind("amq.topic", ex =>
            {
                ex.ExchangeType = "topic";
                ex.RoutingKey = "KK.EVENT.CLIENT.ticketbuddy.SUCCESS.ticketbuddy-ui.REGISTER";
            });
        }
    }
}