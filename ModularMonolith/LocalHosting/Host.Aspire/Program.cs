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

builder.AddViteApp(name: "User-Interface", workingDirectory: "../../../UI")
    .WithReference(api)
    .WaitFor(api)
    .WithReference(dataSeeder)
    .WaitFor(dataSeeder)
    .WithNpmPackageInstallation();

var app = builder.Build();
await app.RunAsync();
