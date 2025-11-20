namespace Integration.Keycloak.Users.Messaging;

public record UserRegistered(Guid userId, Dictionary<string, string> details);