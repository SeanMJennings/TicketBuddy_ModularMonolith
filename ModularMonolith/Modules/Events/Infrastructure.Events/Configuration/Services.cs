using Application.Events.Commands;
using Application.Events.MessageHandlers;
using Application.Events.Queries;
using Domain.Events.Contracts;
using Domain.Events.Services;
using Infrastructure.Events.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureEventsServices(this IServiceCollection services)
    {
        services.AddScoped<IEventsUnitOfWork, UnitOfWork>()
            .AddScoped<IPersistEvents, EventRepository>()
            .AddScoped<EventSoldOutHandler>()
            .AddScoped<EventsValidator>()
            .AddScoped<EventCommands>()
            .AddScoped<EventQueries>();
        return services;
    }
}