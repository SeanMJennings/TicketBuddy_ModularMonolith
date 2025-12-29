using Domain.Tickets.User;
using Infrastructure.Tickets.Core;

namespace Infrastructure.Tickets.User;

public class UserRepository(TicketDbContext ticketDbContext) : IPersistUsers
{
    public async Task Upsert(Domain.Tickets.User.User theUser)
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

    private async Task<Domain.Tickets.User.User?> Get(Guid id)
    {
        return await ticketDbContext.Users.FindAsync(id);
    }
}

