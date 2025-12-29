using Domain.Tickets.Core;
using Domain.Tickets.User;
using Messaging.Keycloak.Users;

namespace Application.Tickets.User;

public class UpsertUser(IPersistUsers userRepository, ITicketsUnitOfWork unitOfWork)
{
    public async Task Execute(UserRegistered message)
    {
        var user = new Domain.Tickets.User.User(
            message.userId,
            new Name($"{message.details["first_name"]} {message.details["last_name"]}"),
            new Email(message.details["email"])
        );
        
        await userRepository.Upsert(user);
        await unitOfWork.Commit();
    }
}

