using Application.Users;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Users.Contracts;
using Infrastructure.Users.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Users.Configuration;

public static class Services
{
    public static void ConfigureUsersServices(this IServiceCollection services)
    {
        services.AddScoped<IAmAUserRepository, UserRepository>();
        services.AddScoped<UserCommands>();
        services.AddScoped<UserQueries>();
    }
}