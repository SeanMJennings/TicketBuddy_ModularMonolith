using Application.Events;
using Domain.Events.Contracts;
using Infrastructure.Events.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Configuration;

public static class Services
{
    public static void ConfigureEventsServices(this IServiceCollection services)
    {
        services.AddScoped<IAmAnEventRepository, EventRepository>();
        services.AddScoped<EventService>();
    }
}