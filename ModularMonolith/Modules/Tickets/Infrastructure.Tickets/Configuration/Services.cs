using Application.Tickets.Commands;
using Application.Tickets.Contracts;
using Application.Tickets.DomainEventHandlers;
using Application.Tickets.Queries;
using Domain.Tickets.Contracts;
using Domain.Tickets.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static IServiceCollection ConfigureTicketsServices(this IServiceCollection services)
    {
        services.AddScoped<IPersistEvents, Commands.EventRepository>()
            .AddScoped<IPersistUsers, Commands.UserRepository>()
            .AddScoped<IQueryTickets, Queries.TicketQuerist>()
            .AddScoped<TicketCommands>()
            .AddScoped<TicketQueries>()
            .AddScoped<TicketsValidator>()
            .AddScoped<AllTicketsSoldHandler>()
            .AddSingleton(DomainEventsToHandlersMap.Map);
        return services;
    }
}