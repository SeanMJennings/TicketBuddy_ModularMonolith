using Migrations;
using Testcontainers.PostgreSql;

namespace Testing.Containers;

public static class PostgreSql
{
    public static PostgreSqlContainer CreateContainer(bool reuse, string label, int port = 1434)
    {
        return new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(port, true)
            .WithLabel("ticketbuddy.suite", label)
            .WithReuse(reuse)
            .Build();
    }
    
    public static void Migrate(this PostgreSqlContainer container)
    {
        Migration.Upgrade(container.GetConnectionString());
    }
}