using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Integration.Users.Messaging.Messages;
using MassTransit;

namespace Application.Tickets.IntegrationMessageConsumers
{
    public class UserConsumer(IAmAUserRepository userRepository) : IConsumer<UserUpserted>
    {
        public async Task Consume(ConsumeContext<UserUpserted> context)
        {
            await userRepository.Save(User.Create(context.Message.Id, context.Message.FullName, context.Message.Email));
        }
    }
    
    public class UserConsumerDefinition : ConsumerDefinition<UserConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<UserConsumer> consumerConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000));
        }
    }
}