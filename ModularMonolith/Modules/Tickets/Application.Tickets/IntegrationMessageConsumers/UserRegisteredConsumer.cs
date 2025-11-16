using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Domain.Tickets.Primitives;
using Integration.Keycloak.Users.Messaging;
using MassTransit;

namespace Application.Tickets.IntegrationMessageConsumers;

public class UserRegisteredConsumer(IPersistUsers UserRepository) : IConsumer<UserRegistered>
{
    public async Task Consume(ConsumeContext<UserRegistered> context)
    {
        var user = User.Create(
            context.Message.userId,
            new Name($"{context.Message.details["first_name"]} {context.Message.details["last_name"]}"),
            new Email(context.Message.details["email"])
        );
        
        await UserRepository.Save(user);
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