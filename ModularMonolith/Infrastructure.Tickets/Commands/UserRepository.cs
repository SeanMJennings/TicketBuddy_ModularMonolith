﻿using Domain.Tickets.Contracts;
using Domain.Tickets.Entities;

namespace Infrastructure.Tickets.Commands;

public class UserRepository(TicketDbContext ticketDbContext) : IAmAUserRepository
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

        await ticketDbContext.SaveChangesAsync();
    }

    private async Task<User?> Get(Guid id)
    {
        return await ticketDbContext.Users.FindAsync(id);
    }
}