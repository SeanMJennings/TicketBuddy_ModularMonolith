using Application.Tickets.Event;
using Application.Tickets.Ticket;
using Application.Tickets.User;
using Domain.Tickets.Core;
using Domain.Tickets.Event;
using Domain.Tickets.Ticket;
using Domain.Tickets.User;
using Microsoft.Extensions.DependencyInjection;
using EventUpsertedHandler = Domain.Tickets.Event.EventUpsertedHandler;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureTicketsServices(this IServiceCollection services)
    {
        services
            // Core
            .AddScoped<ITicketsUnitOfWork, Core.UnitOfWork>()
            // Event slice
            .AddScoped<IPersistEvents, Event.EventRepository>()
            .AddScoped<SyncEvent>()
            .AddScoped<EventUpsertedHandler>()
            // Ticket slice
            .AddScoped<IPersistTickets, Ticket.TicketRepository>()
            .AddScoped<IPersistTicketReservationCache, Ticket.TicketReservationCacheRepository>()
            .AddScoped<IQueryTickets, Ticket.TicketQuerist>()
            .AddScoped<IQueryTicketReservationStatus, Ticket.TicketReservationCacheRepository>()
            .AddScoped<PurchaseTickets>()
            .AddScoped<ReserveTickets>()
            .AddScoped<GetTicketsForEvent>()
            .AddScoped<GetTicketsForUser>()
            .AddScoped<AllTicketsSoldHandler>()
            // User slice
            .AddScoped<IPersistUsers, User.UserRepository>()
            .AddScoped<SyncUser>()
            .AddSingleton(
                TicketDomainEventsToHandlersMap.Map
                    .Concat(DomainEventsToHandlersMap.Map)
                    .ToDictionary(kv => kv.Key, kv => kv.Value));
        return services;
    }
}