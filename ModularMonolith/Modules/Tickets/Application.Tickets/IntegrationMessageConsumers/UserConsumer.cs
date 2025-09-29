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
            await userRepository.Save(new User(context.Message.Id, context.Message.FullName, context.Message.Email));
        }
    }
}