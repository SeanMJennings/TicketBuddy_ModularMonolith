using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;
using Domain.Tickets.ValueObjects;
using Integration.Keycloak.Users.Messaging;
using MassTransit;

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
        
        await userRepository.Save(user);
        await unitOfWork.Commit();
    }
}