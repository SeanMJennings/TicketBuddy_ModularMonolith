using Application.Tickets.User;
using MassTransit;
using Messaging.Keycloak.Users;

namespace Messaging.Tickets.Consumers;

public class UserRegisteredConsumer(SyncUser syncUser) : IConsumer<UserRegistered>
{
    public async Task Consume(ConsumeContext<UserRegistered> context)
    {
        await syncUser.Execute(context.Message);
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