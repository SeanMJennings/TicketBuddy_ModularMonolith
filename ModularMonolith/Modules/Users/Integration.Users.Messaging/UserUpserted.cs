namespace Integration.Users.Messaging;

public record UserUpserted
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = null!;
    public string Email { get; init; } = null!;
}