using Microsoft.Extensions.Configuration;

namespace Dataseeder.Hosting;

internal class Settings
{
    private static IConfiguration Configuration = null!;
    internal ApiSettings Api => new();
    internal KeycloakSettings Keycloak => new();
    internal RabbitMqSettings RabbitMq => new();
   
    internal Settings(IConfiguration theConfiguration)
    {
        Configuration = theConfiguration;
    }
    
    internal class ApiSettings
    {
        public Uri BaseUrl => new(Configuration.GetRequired("ApiSettings:BaseUrl"));
    }
    
    internal class KeycloakSettings
    {
        public Uri BaseUrl => new(Configuration.GetRequired("KeycloakSettings:BaseUrl"));
        public string AdminCliClientId => Configuration.GetRequired("KeycloakSettings:AdminCliClientId");
        public string TicketBuddyApiClientId => Configuration.GetRequired("KeycloakSettings:TicketBuddyApiClientId");
        public string AdminUsername => Configuration.GetRequired("KeycloakSettings:Username");
        public string AdminPassword => Configuration.GetRequired("KeycloakSettings:Password");
        public string MasterRealm => Configuration.GetRequired("KeycloakSettings:MasterRealm");
        public string TicketBuddyRealm => Configuration.GetRequired("KeycloakSettings:TicketBuddyRealm");
    }
    
    internal class RabbitMqSettings
    {
        internal Uri ConnectionString => new(Configuration.GetRequired("ConnectionStrings:Messaging"));
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