using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Users.Configuration;

namespace Api.Hosting;

public static class Services
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureUsersServices();
        services.ConfigureEventsServices();
        services.ConfigureTicketsServices();
    }
}