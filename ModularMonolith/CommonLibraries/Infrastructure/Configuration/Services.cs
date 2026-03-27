using Application;
using Infrastructure.Commands;
using Infrastructure.DomainEventsDispatching;
using Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<DomainEventsDispatcher>();
        services.AddScoped<DomainEventsMapper>();
        services.AddScoped<IPublishMessages, PublishMessages>();
        services.TryAddScoped<IOutboxFlusher, NoOpOutboxFlusher>();
        return services;
    }
}