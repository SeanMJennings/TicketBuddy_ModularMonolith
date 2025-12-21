using Application.Tickets.Commands;
using Application.Tickets.Contracts;
using Application.Tickets.DomainEventHandlers;
using Application.Tickets.Queries;
using Domain.Contracts;
using Domain.Tickets.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureTicketsServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistEvents, Commands.EventRepository>()
            .AddScoped<IPersistTickets, Commands.TicketRepository>()
            .AddScoped<IPersistUsers, Commands.UserRepository>()
            .AddScoped<IUnitOfWork, Commands.UnitOfWork>()
            .AddScoped<IQueryTickets, Queries.TicketQuerist>()
            .AddScoped<TicketCommands>()
            .AddScoped<TicketQueries>()
            .AddScoped<AllTicketsSoldHandler>()
            .AddSingleton(ApplicationLevelDomainEventsToHandlersMap.Map);
        return services;
    }
}