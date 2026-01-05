namespace TicketBuddy.AppHost.Infrastructure;

public static class PostgresResourceExtensions
{
    private const string ResourceName = "Postgres";
    private const string DatabaseName = "TicketBuddy";
    private const string VolumeName = "TicketBuddy.Monolith.Postgres";
    private const int HostPort = 5432;
    private const string DefaultPassword = "YourStrong@Passw0rd";

    public static IResourceBuilder<PostgresDatabaseResource> AddPostgresDatabase(
        this IDistributedApplicationBuilder builder)
    {
        var postgres = builder
            .AddPostgres(ResourceName)
            .WithPassword(builder.AddParameter("PostgresPassword", DefaultPassword))
            .WithDataVolume(VolumeName)
            .WithHostPort(HostPort)
            .WithLifetime(ContainerLifetime.Persistent);

        return postgres.AddDatabase(DatabaseName);
    }
}