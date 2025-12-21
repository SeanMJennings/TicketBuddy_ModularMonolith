using Application.Events.Commands;
using Application.Events.Queries;
using Domain.Contracts;
using Domain.Events.Contracts;
using Domain.Events.Services;
using Infrastructure.Events.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureEventsServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IPersistEvents, EventRepository>()
            .AddScoped<EventsValidator>()
            .AddScoped<EventCommands>()
            .AddScoped<EventQueries>();
        return services;
    }
}