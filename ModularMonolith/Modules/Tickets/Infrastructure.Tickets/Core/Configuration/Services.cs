using Application.Tickets.Core;
using Application.Tickets.Event;
using Application.Tickets.Ticket;
using Application.Tickets.Ticket.GetTicketsForEvent;
using Application.Tickets.Ticket.GetTicketsForUser;
using Application.Tickets.Ticket.PurchaseTickets;
using Application.Tickets.Ticket.ReserveTickets;
using Application.Tickets.User;
using Domain.Tickets.Core;
using Domain.Tickets.Event;
using Domain.Tickets.Ticket;
using Domain.Tickets.User;
using Microsoft.Extensions.DependencyInjection;
using EventUpsertedHandler = Domain.Tickets.Event.EventUpsertedHandler;

namespace Infrastructure.Tickets.Core.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureTicketsServices(this IServiceCollection services)
    {
        services
            .AddScoped<ITicketsUnitOfWork, UnitOfWork>()
            .AddScoped<IPersistEvents, Event.EventRepository>()
            .AddScoped<UpsertEvent>()
            .AddScoped<EventUpsertedHandler>()
            .AddScoped<IPersistTickets, Ticket.TicketRepository>()
            .AddScoped<IQueryTickets, Ticket.TicketQuerist>()
            .AddScoped<IQueryTicketReservations, Ticket.TicketReservationCacheRepository>()
            .AddScoped<IExtendTicketsInTheReservationCache, Ticket.TicketReservationCacheRepository>()
            .AddScoped<PurchaseTickets>()
            .AddScoped<ReserveTickets>()
            .AddScoped<GetTicketsForEvent>()
            .AddScoped<GetTicketsForUser>()
            .AddScoped<AllTicketsSoldHandler>()
            .AddScoped<IPersistUsers, User.UserRepository>()
            .AddScoped<UpsertUser>()
            .AddSingleton(
                TicketDomainEventsToHandlersMap.Map
                    .Concat(DomainEventsToHandlersMap.Map)
                    .ToDictionary(kv => kv.Key, kv => kv.Value));
        return services;
    }
}