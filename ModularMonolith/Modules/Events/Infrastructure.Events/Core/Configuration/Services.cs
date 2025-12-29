using Application.Events;
using Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events.Core.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureEventsServices(this IServiceCollection services)
    {
        services
            .AddScoped<IEventsUnitOfWork, UnitOfWork>()
            .AddScoped<IPersistEvents, Event.EventRepository>()
            .AddScoped<EventsValidator>()
            .AddScoped<CreateEvent>()
            .AddScoped<UpdateEvent>()
            .AddScoped<GetEvents>()
            .AddScoped<GetEventById>()
            .AddScoped<MarkEventAsSoldOut>();
        return services;
    }
}