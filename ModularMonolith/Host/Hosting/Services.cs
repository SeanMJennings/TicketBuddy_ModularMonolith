using Infrastructure.Configuration;
using Infrastructure.Events.Core.Configuration;
using Infrastructure.Notifications.Core.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Tickets.Core.Configuration;

namespace Api.Hosting;

public static class Services
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureInfrastructureServices();
        services.ConfigureEventsServices();
        services.ConfigureTicketsServices();
        services.ConfigureNotificationsServices();
    }
}