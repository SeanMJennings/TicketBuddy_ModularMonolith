using Api.Hosting;
using Microsoft.Extensions.Configuration;

namespace Migrations.Host;

public static class Settings
{
    private static readonly IConfiguration configuration = Configuration.Build();

    public static class Database
    {
        public static string Connection => configuration["ConnectionStrings:TicketBuddy"]!;
    }
}