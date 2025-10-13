using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Acceptance;

public class IntegrationWebApplicationFactory<TProgram>(string connectionString, string redisConnectionString, string rabbitMqConnectionString)
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    { 
        Environment.SetEnvironmentVariable("ConnectionStrings__TicketBuddy", connectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__Cache", redisConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__Messaging", rabbitMqConnectionString);
    }
}