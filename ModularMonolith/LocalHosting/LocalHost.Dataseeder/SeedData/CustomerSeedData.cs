namespace Dataseeder.SeedData;

internal static class CustomerSeedData
{
    internal static readonly IReadOnlyList<CustomerData> Customers =
    [
        new(Guid.CreateVersion7(), "John", "Smith", "john.smith@example.com"),
        new(Guid.CreateVersion7(), "Jane", "Doe", "jane.doe@example.com"),
        new(Guid.CreateVersion7(), "Robert", "Johnson", "robert.johnson@example.com"),
        new(Guid.CreateVersion7(), "Emily", "Davis", "emily.davis@example.com")
    ];
}

internal record CustomerData(Guid UserId, string FirstName, string LastName, string Email)
{
    public string Password => $"{FirstName.ToLowerInvariant()}{LastName.ToLowerInvariant()}";
}
