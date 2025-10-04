using System.ComponentModel.DataAnnotations;
using Domain.Users.Contracts;
using Domain.Users.Entities;
using Domain.Users.Primitives;

namespace Application.Users.Commands;

public class UserCommands(IAmAUserRepository UserRepository)
{
    public async Task<Guid> CreateUser(FullName fullName, Email email, UserType userType)
    {
        var id = Guid.NewGuid();
        var user = new User(id, fullName, email, userType);
        if (await IsEmailAlreadyUsedByOtherUser(user.Id, user.Email)) throw new ValidationException("Email already exists");

        await UserRepository.Add(user);
        await UserRepository.Commit();
        return user.Id;
    }

    public async Task UpdateUser(Guid id, FullName fullName, Email email)
    {
        if (await IsEmailAlreadyUsedByOtherUser(id, email)) throw new ValidationException("Email already exists");
        var existingUser = await UserRepository.Get(id);

        if (existingUser is null) throw new ValidationException("User does not exist");
        
        existingUser.UpdateName(fullName);
        existingUser.UpdateEmail(email);
        await UserRepository.Update(existingUser);
        await UserRepository.Commit();
    }
    
    private async Task<bool> IsEmailAlreadyUsedByOtherUser(Guid userId, string email)
    {
        return (await UserRepository.GetAll()).Any(u => u.Email == email && u.Id != userId);
    }
}