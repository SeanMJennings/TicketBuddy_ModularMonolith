using Infrastructure.Tickets.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheDatabase = Infrastructure.Queries.Database;

namespace Infrastructure.Tickets.Configuration;

public static class Database
{
    public static void ConfigureTicketsDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TicketDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });
        services.AddScoped<TheDatabase>(_ => new TheDatabase(connectionString));
    }
}