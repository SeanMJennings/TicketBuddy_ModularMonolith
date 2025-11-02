using Migrations;
using Testcontainers.PostgreSql;

namespace Testing.Containers;

public static class PostgreSql
{
    public static PostgreSqlContainer CreateContainer(int port = 1434)
    {
        return new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(port, true)
            .Build();
    }
    
    public static void Migrate(this PostgreSqlContainer container)
    {
        Migration.Upgrade(container.GetConnectionString());
    }
}