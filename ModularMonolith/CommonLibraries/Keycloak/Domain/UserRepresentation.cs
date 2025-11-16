namespace Keycloak.Domain;

public class UserRepresentation
{
    public string firstName { get; init; }
    public string lastName { get; init; }
    public string email { get; init; }
    public string[] realmRoles { get; } = ["default-roles-ticketbuddy"];
    public bool emailVerified { get; } = true;
    public bool enabled { get; } = true;
    public int notBefore { get; } = 0;
    public List<CredentialRepresentation> credentials { get; init; }
}