using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Notifications.Core.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureNotificationsServices(this IServiceCollection services)
    {
        return services;
    }
}