using Infrastructure.Events.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Configuration;

public static class Database
{
    public static void ConfigureEventsDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<EventDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
    }
}