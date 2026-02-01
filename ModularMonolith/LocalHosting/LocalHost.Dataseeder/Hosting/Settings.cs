using Microsoft.Extensions.Configuration;

namespace Dataseeder.Hosting;

internal class Settings
{
    private static IConfiguration _configuration = null!;
    internal ApiSettings Api => new();
    internal KeycloakSettings Keycloak => new();
    internal RabbitMqSettings RabbitMq => new();
   
    internal Settings(IConfiguration theConfiguration)
    {
        _configuration = theConfiguration;
    }
    
    internal class ApiSettings
    {
        internal Uri BaseUrl => new(_configuration.GetRequired("ApiSettings:BaseUrl"));
    }
    
    internal class KeycloakSettings
    {
        internal Uri BaseUrl => new(_configuration.GetRequired("KeycloakSettings:BaseUrl"));
        internal string AdminCliClientId => _configuration.GetRequired("KeycloakSettings:AdminCliClientId");
        internal string TicketBuddyApiClientId => _configuration.GetRequired("KeycloakSettings:TicketBuddyApiClientId");
        internal string AdminUsername => _configuration.GetRequired("KeycloakSettings:Username");
        internal string AdminPassword => _configuration.GetRequired("KeycloakSettings:Password");
        internal string MasterRealm => _configuration.GetRequired("KeycloakSettings:MasterRealm");
        internal string TicketBuddyRealm => _configuration.GetRequired("KeycloakSettings:TicketBuddyRealm");
    }
    
    internal class RabbitMqSettings
    {
        internal Uri ConnectionString => new(_configuration.GetRequired("ConnectionStrings:Messaging"));
    }
}

internal static class ConfigurationExtensions
{
    internal static string GetRequired(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        return string.IsNullOrEmpty(value) ? throw new InvalidOperationException($"Configuration key '{key}' is required but was not found.") : value;
    }
}