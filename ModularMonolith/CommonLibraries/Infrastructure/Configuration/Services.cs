using Application;
using Infrastructure.DomainEventsDispatching;
using Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<DomainEventsDispatcher>();
        services.AddScoped<DomainEventsMapper>();
        services.AddScoped<IPublishMessages, PublishMessages>();
        return services;
    }
}