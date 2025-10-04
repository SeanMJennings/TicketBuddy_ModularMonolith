using Application.Events;
using Application.Events.Commands;
using Application.Events.Queries;
using Domain.Events.Contracts;
using Infrastructure.Events.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Configuration;

public static class Services
{
    public static void ConfigureEventsServices(this IServiceCollection services)
    {
        services.AddScoped<IAmAnEventRepository, EventRepository>();
        services.AddScoped<EventCommands>();
        services.AddScoped<EventQueries>();
    }
}