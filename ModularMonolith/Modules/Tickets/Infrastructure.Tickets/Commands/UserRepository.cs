using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;

namespace Infrastructure.Tickets.Commands;

public class UserRepository(TicketDbContext ticketDbContext) : IPersistUsers
{
    public async Task Save(User theUser)
    {
        var existingUser = await Get(theUser.Id);
        if (existingUser is not null)
        {
            existingUser.UpdateName(theUser.FullName);
            existingUser.UpdateEmail(theUser.Email);
            ticketDbContext.Update(existingUser);
        }
        else
        {
            ticketDbContext.Add(theUser);
        }
    }

    private async Task<User?> Get(Guid id)
    {
        return await ticketDbContext.Users.FindAsync(id);
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await ticketDbContext.Commit(cancellationToken);
    }
}