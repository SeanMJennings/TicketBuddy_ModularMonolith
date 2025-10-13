using System.Text.Json.Serialization;
using Domain.Events;
using Domain.Users;
using Infrastructure.Tickets.Configuration;
using OpenTelemetry;
using WebHost;

namespace Api.Hosting;

internal sealed class Api(WebApplicationBuilder webApplicationBuilder, IConfiguration configuration) : WebApi(webApplicationBuilder, configuration)
{
    private readonly Settings _settings = new(configuration);
    protected override string ApplicationName => nameof(Api);
    protected override string TelemetryConnectionString => _settings.Telemetry.ConnectionString;

    protected override List<JsonConverter> JsonConverters => EventsConverters.GetConverters.Concat(UsersConverters.GetConverters).ToList();

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.ConfigureDatabase(_settings.Database.Connection);
        services.ConfigureCache(_settings.Cache.Connection);
        services.ConfigureServices();
        if (!string.IsNullOrEmpty(_settings.RabbitMq.ConnectionString.ToString())) services.ConfigureMessaging(_settings.RabbitMq.ConnectionString.ToString());
        services.ConfigureHealthChecks(_settings.Database.Connection, _settings.Cache.Connection, _settings.RabbitMq.ConnectionString.ToString());
        services.AddCorsAllowAll();
    }

    protected override OpenTelemetryBuilder ConfigureTelemetry(WebApplicationBuilder builder)
    {
        var otel = base.ConfigureTelemetry(builder);
        return otel.WithTelemetry(_settings, ApplicationName);
    }

    protected override void ConfigureApplication(WebApplication theApp)
    {
        base.ConfigureApplication(theApp);
        theApp.UseHealthChecks("/health");
        theApp.UseCorsAllowAll();
    }
}