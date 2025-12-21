using Application.Tickets.Contracts;
using Domain.Contracts;
using Domain.Tickets.Entities;
using Domain.Tickets.ValueObjects;
using Integration.Keycloak.Users.Messaging;
using MassTransit;

namespace Application.Tickets.IntegrationMessageConsumers;

public class UserRegisteredConsumer(IPersistUsers userRepository, IUnitOfWork unitOfWork) : IConsumer<UserRegistered>
{
    public async Task Consume(ConsumeContext<UserRegistered> context)
    {
        var user = User.Create(
            context.Message.userId,
            new Name($"{context.Message.details["first_name"]} {context.Message.details["last_name"]}"),
            new Email(context.Message.details["email"])
        );
        
        await userRepository.Save(user);
        await unitOfWork.Commit(context.CancellationToken);
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