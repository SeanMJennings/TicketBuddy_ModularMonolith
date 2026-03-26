namespace TicketBuddy.AppHost.Infrastructure;

public static class RedisResourceExtensions
{
    private const string ResourceName = "Cache";
    private const string VolumeName = "Ticketbuddy.Aspire.Redis";
    private const string Image = "redis:latest";
    private const string DefaultPassword = "YourStrong@Passw0rd";
    private const int Port = 6379;

    public static IResourceBuilder<RedisResource> AddRedisCache(
        this IDistributedApplicationBuilder builder)
    {
        return builder
            .AddRedis(ResourceName, Port)
            .WithImage(Image)
            .WithDataVolume(VolumeName)
            .WithPassword(builder.AddParameter("RedisPassword", DefaultPassword))
            .WithLifetime(ContainerLifetime.Persistent)
            .WithComposeProjectLabel();
    }
}