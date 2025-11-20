namespace Keycloak.Domain;

public class CredentialRepresentation
{
    public string userLabel { get; } = "default";
    public bool temporary { get; } = false;
    public string type { get; } = "password";
    public string value { get; init; } = string.Empty;
}