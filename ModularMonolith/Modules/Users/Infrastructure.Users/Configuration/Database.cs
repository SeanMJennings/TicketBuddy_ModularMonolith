using Infrastructure.Users.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Users.Configuration;

public static class Database
{
    public static void ConfigureUsersDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
    }
}