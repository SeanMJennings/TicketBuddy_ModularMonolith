using Dataseeder.Infrastructure;
using Dataseeder.SeedData;
using Keycloak.Requests;

namespace Dataseeder.Seeders;

internal class UserSeeder(KeycloakApiClient keycloakApiClient, KeycloakEventPublisher eventPublisher)
{
    internal async Task SeedAsync()
    {
        foreach (var customer in CustomerSeedData.Customers)
        {
            var payload = new UserRepresentation
            {
                id = customer.UserId,
                firstName = customer.FirstName,
                lastName = customer.LastName,
                email = customer.Email,
                credentials =
                [
                    new CredentialRepresentation
                    {
                        value = customer.Password
                    }
                ]
            };

            await keycloakApiClient.CreateUserAsync(payload);
            await eventPublisher.PublishUserRegisteredAsync(
                customer.UserId,
                customer.FirstName,
                customer.LastName,
                customer.Email);
        }
    }
}