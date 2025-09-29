using Common.Environment;
using Microsoft.Extensions.Configuration;

namespace Dataseeder.Hosting;

public static class Configuration
{
    public static IConfigurationRoot Build()
    {
        var environment = CommonEnvironmentExtensions.GetEnvironment();
        
        if (environment is CommonEnvironment.LocalDevelopment)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.local.json", optional: false, reloadOnChange: false);
            return configurationBuilder.Build();  
        }

        var theConfigurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.docker.json", optional: false, reloadOnChange: false);
        return theConfigurationBuilder.Build();
    }
}