using Application.Events;
using Application.Events.Venue;
using Domain.Events;
using Domain.Events.Venue;
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
            .AddScoped<MarkEventAsSoldOut>()
            .AddScoped<IPersistVenues, Venue.VenueRepository>()
            .AddScoped<VenuesValidator>()
            .AddScoped<CreateVenue>()
            .AddScoped<UpdateVenue>()
            .AddScoped<GetVenueById>()
            .AddScoped<GetVenues>();
        return services;
    }
}