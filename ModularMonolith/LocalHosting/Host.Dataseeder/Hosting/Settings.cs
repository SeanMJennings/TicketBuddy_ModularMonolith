using Microsoft.Extensions.Configuration;

namespace Dataseeder.Hosting;

internal class Settings
{
    private static IConfiguration Configuration = null!;
    internal ApiSettings Api => new();
   
    internal Settings(IConfiguration theConfiguration)
    {
        Configuration = theConfiguration;
    }
    
    internal class ApiSettings
    {
        public Uri BaseUrl => new Uri(Configuration["ApiSettings:BaseUrl"]!);
    }
}