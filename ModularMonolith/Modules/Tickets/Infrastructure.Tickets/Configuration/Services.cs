using Application.Tickets.Commands;
using Application.Tickets.Contracts;
using Application.Tickets.DomainEventHandlers;
using Application.Tickets.Queries;
using Domain.Tickets.Contracts;
using Domain.Tickets.DomainEventHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureTicketsServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistEvents, Commands.EventRepository>()
            .AddScoped<IPersistTickets, Commands.TicketRepository>()
            .AddScoped<IPersistUsers, Commands.UserRepository>()
            .AddScoped<ITicketsUnitOfWork, Commands.UnitOfWork>()
            .AddScoped<IQueryTickets, Queries.TicketQuerist>()
            .AddScoped<TicketCommands>()
            .AddScoped<TicketQueries>()
            .AddScoped<AllTicketsSoldHandler>()
            .AddScoped<EventUpsertedHandler>()
            .AddSingleton(ApplicationLevelDomainEventsToHandlersMap.Map.Concat(DomainEventsToHandlersMap.Map).ToDictionary(kv => kv.Key, kv => kv.Value));
        return services;
    }
}