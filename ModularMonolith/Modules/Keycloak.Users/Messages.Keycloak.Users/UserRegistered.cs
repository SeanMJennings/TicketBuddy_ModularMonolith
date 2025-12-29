namespace Messaging.Keycloak.Users;

public record UserRegistered(Guid userId, Dictionary<string, string> details);