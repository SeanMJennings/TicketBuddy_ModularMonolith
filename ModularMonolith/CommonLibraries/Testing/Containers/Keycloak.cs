using Testcontainers.Keycloak;

namespace Testing.Containers;

public static class Keycloak
{
    public static KeycloakContainer CreateContainer(int port = 8181)
    {
        return new KeycloakBuilder()
            .WithPortBinding(port, true)
            .WithRealm("ticketbuddy-realm.json")
            .WithUsername("admin")
            .WithPassword("admin")
            .Build();
    }
}