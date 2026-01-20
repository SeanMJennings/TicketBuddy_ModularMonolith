using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Notifications.Core.Configuration;

public static class Database
{
    public static IServiceCollection ConfigureNotificationsDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<NotificationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
        return services;
    }
}