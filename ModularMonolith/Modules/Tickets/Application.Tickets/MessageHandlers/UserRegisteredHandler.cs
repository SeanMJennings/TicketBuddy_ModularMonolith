using Domain.Tickets.Core;
using Domain.Tickets.User;
using Messaging.Keycloak.Users;

namespace Application.Tickets.MessageHandlers;

public class UserRegisteredHandler(IPersistUsers userRepository, ITicketsUnitOfWork unitOfWork)
{
    public async Task Handle(UserRegistered message)
    {
        var user = new User(
            message.userId,
            new Name($"{message.details["first_name"]} {message.details["last_name"]}"),
            new Email(message.details["email"])
        );
        
        await userRepository.Upsert(user);
        await unitOfWork.Commit();
    }
}