using Infrastructure.Configuration;
using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;

namespace Api.Hosting;

public static class Services
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureInfrastructureServices();
        services.ConfigureEventsServices();
        services.ConfigureTicketsServices();
    }
}