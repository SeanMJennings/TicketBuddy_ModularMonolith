using Application.Tickets.Commands;
using Application.Tickets.DomainEventHandlers;
using Application.Tickets.Queries;
using Domain.Tickets.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureTicketsServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistEvents, Commands.EventRepository>();
        services.AddScoped<IPersistUsers, Commands.UserRepository>();
        services.AddScoped<IQueryTickets, Queries.TicketQuerist>();
        services.AddScoped<TicketCommands>();
        services.AddScoped<TicketQueries>();
        services.AddScoped<AllTicketsSoldHandler>();
        services.AddSingleton(DomainEventsToHandlersMap.Map);
        return services;
    }
}