using Migrations;
using Testcontainers.PostgreSql;

namespace Testing.Containers;

public static class PostgreSql
{
    public static PostgreSqlContainer CreateContainer()
    {
        return new PostgreSqlBuilder()
            .WithDatabase("TicketBuddy")
            .WithUsername("sa")
            .WithPassword("yourStrong(!)Password")
            .WithPortBinding(1434, true)
            .Build();
    }
    
    public static void Migrate(this PostgreSqlContainer container)
    {
        Migration.Upgrade(container.GetConnectionString());
    }
}