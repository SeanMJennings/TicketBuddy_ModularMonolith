using Common.Environment;

const string Environment = "Environment";
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("Postgres")
    .WithPassword(builder.AddParameter("PostgresPassword", "YourStrong@Passw0rd"))
    .WithDataVolume("TicketBuddy.Monolith.Postgres")
    .WithHostPort(5432)
    .WithLifetime(ContainerLifetime.Persistent);

var database = postgres.AddDatabase("TicketBuddy");

var rabbitUserParam = builder.AddParameter("RabbitMQUsername", "guest", secret: true);
var rabbitPasswordParam = builder.AddParameter("RabbitMQPassword", "guest", secret: true);
var rabbitmq = builder
    .AddRabbitMQ("Messaging",
        userName: rabbitUserParam,
        password: rabbitPasswordParam)
    .WithImage("masstransit/rabbitmq")
    .WithDataVolume("TicketBuddy.Monolith.RabbitMQ")
    .WithHttpEndpoint(port: 5672, targetPort: 5672)
    .WithHttpsEndpoint(port: 15672, targetPort: 15672)
    .WithLifetime(ContainerLifetime.Persistent);

var redis = builder
    .AddRedis("Cache", 6379)
    .WithImage("redis:7.0-alpine")
    .WithDataVolume("TicketBuddy.Monolith.Redis")
    .WithPassword(builder.AddParameter("RedisPassword", "YourStrong@Passw0rd"))
    .WithLifetime(ContainerLifetime.Persistent);

var keycloakJarHostPath = Path.Combine(AppContext.BaseDirectory, "keycloak-to-rabbit-3.0.5.jar");
var kkToRmqUrlParam = builder.AddParameter("RabbitMQUrl", "Messaging");
var kkToRmqVhostParam = builder.AddParameter("RabbitMQVHost", "/");

var keycloak = builder
    .AddKeycloak("Identity", 8180,
        adminUsername: builder.AddParameter("KeycloakAdminUsername", "admin"), 
        adminPassword: builder.AddParameter("KeycloakAdminPassword", "admin"))
    .WithDataVolume("TicketBuddy.Monolith.Identity")
    .WithRealmImport(Path.Combine(AppContext.BaseDirectory, "ticketbuddy-realm.json"))
    .WithBindMount(keycloakJarHostPath, "/opt/keycloak/providers/keycloak-to-rabbit-3.0.5.jar")
    .WithEnvironment("KK_TO_RMQ_URL", kkToRmqUrlParam)
    .WithEnvironment("KK_TO_RMQ_VHOST", kkToRmqVhostParam)
    .WithEnvironment("KK_TO_RMQ_USERNAME", rabbitUserParam)
    .WithEnvironment("KK_TO_RMQ_PASSWORD", rabbitPasswordParam)
    .WithLifetime(ContainerLifetime.Persistent);

var migrations = builder.AddProject<Projects.Host_Migrations>("Migrations")
    .WithReference(database)
    .WaitFor(database)
    .WithEnvironment(Environment, CommonEnvironment.LocalDevelopment.ToString);

var api = builder.AddProject<Projects.Host>("Api")
    .WithReference(database)
    .WaitFor(database)
    .WithReference(migrations)
    .WaitFor(migrations)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(keycloak)
    .WaitFor(keycloak)
    .WithEnvironment(Environment, CommonEnvironment.LocalDevelopment.ToString);

var dataSeeder = builder.AddProject<Projects.Host_Dataseeder>("Dataseeder")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment(Environment, CommonEnvironment.LocalDevelopment.ToString);

var repoRoot = FindRepoRootContaining(AppContext.BaseDirectory, "UI");
var uiBuildContext = Path.GetFullPath(Path.Combine(repoRoot, "UI"));
var uiDockerfile = Path.GetFullPath(Path.Combine(uiBuildContext, "Dockerfile"));

if (!Directory.Exists(uiBuildContext)) throw new DirectoryNotFoundException($"Could not locate folder `UI` at `{uiBuildContext}`");
if (!File.Exists(uiDockerfile)) throw new FileNotFoundException($"Dockerfile not found at `{uiDockerfile}`");

var imageName = "localhosting-ui:local";
var psi = new System.Diagnostics.ProcessStartInfo("docker",
    $"build -f \"{uiDockerfile}\" -t {imageName} \"{uiBuildContext}\"")
{
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false
};

using (var proc = System.Diagnostics.Process.Start(psi)!)
{
    var stdout = proc.StandardOutput.ReadToEndAsync();
    var stderr = proc.StandardError.ReadToEndAsync();
    proc.WaitForExit();
    if (proc.ExitCode != 0)
    {
        var outText = await stdout;
        var errText = await stderr;
        throw new InvalidOperationException($"Docker build failed for `UI`.\nStdout:\n{outText}\nStderr:\n{errText}");
    }
}

builder
    .AddContainer("User-Interface", imageName)
    .WithHttpEndpoint(port: 5173, targetPort: 5173)
    .WithReference(api)
    .WaitFor(api)
    .WithReference(dataSeeder)
    .WaitFor(dataSeeder)
    .WithLifetime(ContainerLifetime.Persistent);

var app = builder.Build();
await app.RunAsync();

return;

static string FindRepoRootContaining(string startDir, string markerFolder)
{
    var dir = new DirectoryInfo(startDir);
    while (dir != null)
    {
        if (Directory.Exists(Path.Combine(dir.FullName, markerFolder)))
            return dir.FullName;
        dir = dir.Parent;
    }
    throw new DirectoryNotFoundException($"Could not locate folder `{markerFolder}` from `{startDir}` upward.");
}