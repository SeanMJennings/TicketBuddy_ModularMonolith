using Application.Tickets;
using Application.Tickets.DomainEventHandlers;
using Domain.Tickets.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static void ConfigureTicketsServices(this IServiceCollection services)
    {
        services.AddScoped<IAmAnEventRepository, Commands.EventRepository>();
        services.AddScoped<IAmAUserRepository, Commands.UserRepository>();
        services.AddScoped<IQueryTickets, Queries.TicketQuerist>();
        services.AddScoped<TicketService>();
        services.AddScoped<AllTicketsSoldHandler>();
        services.AddSingleton(DomainEventsToHandlersMap.Map);
    }
}