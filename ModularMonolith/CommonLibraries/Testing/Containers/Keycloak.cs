using Testcontainers.Keycloak;

namespace Testing.Containers;

public static class Keycloak
{
    public const string AdminUserName = "admin";
    public const string AdminPassword = "admin";
    public static KeycloakContainer CreateContainer(Uri rabbitMqUrl, int port = 8181)
    {
        var keycloakJarHostPath = Path.Combine(AppContext.BaseDirectory, "keycloak-to-rabbit-3.0.5.jar");
        
        return new KeycloakBuilder()
            .WithImage("quay.io/keycloak/keycloak:26.3")
            .WithPortBinding(port, true)
            .WithRealm("ticketbuddy-realm.json")
            .WithBindMount(keycloakJarHostPath, "/opt/keycloak/providers/keycloak-to-rabbit-3.0.5.jar")
            .WithUsername(AdminUserName)
            .WithPassword(AdminPassword)
            .WithEnvironment("KK_TO_RMQ_URL", rabbitMqUrl.ToString())
            .WithEnvironment("KK_TO_RMQ_VHOST", "/")
            .WithEnvironment("KK_TO_RMQ_USERNAME", RabbitMq.UserName)
            .WithEnvironment("KK_TO_RMQ_PASSWORD", RabbitMq.Password)
            .Build();
    }
}