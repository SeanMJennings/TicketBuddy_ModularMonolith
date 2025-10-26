using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Users.Contracts;
using Infrastructure.Users.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Users.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureUsersServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistUsers, UserRepository>();
        services.AddScoped<UserCommands>();
        services.AddScoped<UserQueries>();
        return services;
    }
}