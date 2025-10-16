namespace Api.Hosting;

internal class Settings
{
    private static IConfiguration Configuration = null!;
    internal CacheSettings Cache => new();
    internal DatabaseSettings Database => new();
    internal RabbitMqSettings RabbitMq => new();
    internal TelemetrySettings Telemetry => new();
   
    internal Settings(IConfiguration theConfiguration)
    {
        Configuration = theConfiguration;
    }
    
    internal class RabbitMqSettings
    {
        internal Uri ConnectionString => new(Configuration["ConnectionStrings:Messaging"]!);
    }
    
    internal class TelemetrySettings
    {
        internal string ConnectionString
        {
            get
            {
                var otelEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
                return !string.IsNullOrEmpty(otelEndpoint) ? otelEndpoint : Configuration["ConnectionStrings:Telemetry"]!;
            }
        }
    }

    internal class DatabaseSettings
    {
        public string Connection => Configuration["ConnectionStrings:TicketBuddy"]!;
    }
    
    internal class CacheSettings
    {
        public string Connection => Configuration["ConnectionStrings:Cache"]!;
    }
}