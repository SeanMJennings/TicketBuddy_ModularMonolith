using Infrastructure.DomainEventsDispatching;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class Services
{
    public static void ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<DomainEventsAccessor>();
        services.AddScoped<DomainEventsDispatcher>();
        services.AddScoped<DomainEventsMapper>();
    }
}