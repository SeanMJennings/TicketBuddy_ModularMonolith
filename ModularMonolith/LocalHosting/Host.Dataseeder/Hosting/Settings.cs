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
        public Uri BaseUrl => new(Configuration["ApiSettings:BaseUrl"]!);
    }
    
    internal class KeycloakSettings
    {
        public Uri BaseUrl => new(Configuration["KeycloakSettings:BaseUrl"]!);
        public string ClientId => Configuration["KeycloakSettings:ClientId"]!;
        public string AdminUsername => Configuration["KeycloakSettings:Username"]!;
        public string AdminPassword => Configuration["KeycloakSettings:Password"]!;
    }
    
    internal class RabbitMqSettings
    {
        internal Uri ConnectionString => new(Configuration["ConnectionStrings:Messaging"]!);
    }
}