namespace Keycloak.Domain;

public class UserRepresentation
{
    public Guid id { get; init; }
    public string firstName { get; init; } = null!;
    public string lastName { get; init; } = null!;
    public string email { get; init; } = null!;
    public string[] realmRoles { get; } = ["default-roles-ticketbuddy"];
    public bool emailVerified { get; } = true;
    public bool enabled { get; } = true;
    public int notBefore { get; } = 0;
    public List<CredentialRepresentation> credentials { get; init; } = [];
}