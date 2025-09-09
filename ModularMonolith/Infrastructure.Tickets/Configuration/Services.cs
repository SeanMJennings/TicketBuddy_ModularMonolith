using Application.Tickets;
using Domain.Tickets.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Tickets.Configuration;

public static class Services
{
    public static void ConfigureTicketsServices(this IServiceCollection services)
    {
        services.AddScoped<IAmAnEventRepository, Commands.EventRepository>();
        services.AddScoped<IAmAUserRepository, Commands.UserRepository>();
        services.AddScoped<IAmATicketRepositoryForCommands, Commands.CommandTicketRepository>();
        services.AddScoped<IAmATicketRepositoryForQueries, Queries.QueryTicketRepository>();
        services.AddScoped<TicketService>();
    }
}