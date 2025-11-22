namespace Api.Hosting;

internal class Settings
{
    private static IConfiguration Configuration = null!;
    internal CacheSettings Cache => new();
    internal DatabaseSettings Database => new();
    internal RabbitMqSettings RabbitMq => new();
    internal KeycloakSettings Keycloak => new();
    internal TelemetrySettings Telemetry => new();
   
    internal Settings(IConfiguration theConfiguration)
    {
        Configuration = theConfiguration;
    }
    
    internal class RabbitMqSettings
    {
        internal Uri ConnectionString => new(Configuration.GetRequired("ConnectionStrings:Messaging"));
    }
    
    internal class TelemetrySettings
    {
        internal string ConnectionString
        {
            get
            {
                var otelEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                return !string.IsNullOrEmpty(otelEndpoint) ? otelEndpoint : Configuration.GetRequired("ConnectionStrings:Telemetry");
            }
        }
    }

    internal class DatabaseSettings
    {
        public string Connection => Configuration.GetRequired("ConnectionStrings:TicketBuddy");
    }
    
    internal class CacheSettings
    {
        public string Connection => Configuration.GetRequired("ConnectionStrings:Cache");
    }
    
    internal class KeycloakSettings
    {
        public string ServerUrl => Configuration.GetRequired("ConnectionStrings:Identity");
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