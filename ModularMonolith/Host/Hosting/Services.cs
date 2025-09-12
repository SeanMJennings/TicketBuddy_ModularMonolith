using Application.Tickets.DomainEventHandlers;
using Infrastructure.Configuration;
using Infrastructure.DomainEventsDispatching;
using Infrastructure.Events.Configuration;
using Infrastructure.Tickets.Configuration;
using Infrastructure.Users.Configuration;

namespace Api.Hosting;

public static class Services
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.ConfigureInfrastructureServices();
        services.ConfigureUsersServices();
        services.ConfigureEventsServices();
        services.ConfigureTicketsServices();
        services.AddSingleton(DomainEventsToHandlersMap.Map);
    }
}