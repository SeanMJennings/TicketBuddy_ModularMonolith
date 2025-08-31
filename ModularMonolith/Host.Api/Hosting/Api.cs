﻿using System.Text.Json.Serialization;
using Domain.Events;
using Domain.Users;
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
        services.ConfigureServices();
        if (IsNotLocalTestingOrBuildPipeline()) services.ConfigureMessaging(_settings);
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
        theApp.UseCorsAllowAll();
    }
}