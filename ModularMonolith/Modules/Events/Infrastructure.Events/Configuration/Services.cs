using Application.Events.Commands;
using Application.Events.Queries;
using Domain.Events.Contracts;
using Infrastructure.Events.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureEventsServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistEvents, EventRepository>();
        services.AddScoped<EventCommands>();
        services.AddScoped<EventQueries>();
        return services;
    }
}