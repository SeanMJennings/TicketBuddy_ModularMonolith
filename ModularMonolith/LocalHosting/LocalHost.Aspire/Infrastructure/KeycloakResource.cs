namespace TicketBuddy.AppHost.Infrastructure;

public static class KeycloakResourceExtensions
{
    private const string ResourceName = "Identity";
    private const string VolumeName = "Ticketbuddy.Aspire.Identity";
    private const string DefaultAdminUsername = "admin";
    private const string DefaultAdminPassword = "admin";
    private const int Port = 8180;

    private const string KeycloakToRabbitJarName = "keycloak-to-rabbit-3.0.5.jar";
    private const string KeycloakProvidersPath = "/opt/keycloak/providers";
    private const string RealmFileName = "ticketbuddy-realm.json";

    public static IResourceBuilder<KeycloakResource> AddKeycloakIdentity(
        this IDistributedApplicationBuilder builder,
        IResourceBuilder<ParameterResource> rabbitUserParam,
        IResourceBuilder<ParameterResource> rabbitPasswordParam)
    {
        var jarHostPath = Path.Combine(AppContext.BaseDirectory, KeycloakToRabbitJarName);
        var rabbitUrlParam = builder.AddParameter("RabbitMQUrl", RabbitMqResourceExtensions.ResourceName);
        var rabbitVHostParam = builder.AddParameter("RabbitMQVHost", "/");

        return builder
            .AddKeycloak(ResourceName, Port,
                adminUsername: builder.AddParameter("KeycloakAdminUsername", DefaultAdminUsername),
                adminPassword: builder.AddParameter("KeycloakAdminPassword", DefaultAdminPassword))
            .WithDataVolume(VolumeName)
            .WithRealmImport(Path.Combine(AppContext.BaseDirectory, RealmFileName))
            .WithBindMount(jarHostPath, $"{KeycloakProvidersPath}/{KeycloakToRabbitJarName}")
            .WithEnvironment("KK_TO_RMQ_URL", rabbitUrlParam)
            .WithEnvironment("KK_TO_RMQ_VHOST", rabbitVHostParam)
            .WithEnvironment("KK_TO_RMQ_USERNAME", rabbitUserParam)
            .WithEnvironment("KK_TO_RMQ_PASSWORD", rabbitPasswordParam)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithComposeProjectLabel();
    }
}