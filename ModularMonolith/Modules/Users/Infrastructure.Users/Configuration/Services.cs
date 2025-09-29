using Application.Users;
using Domain.Users.Contracts;
using Infrastructure.Users.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Users.Configuration;

public static class Services
{
    public static void ConfigureUsersServices(this IServiceCollection services)
    {
        services.AddScoped<IAmAUserRepository, UserRepository>();
        services.AddScoped<UserService>();
    }
}