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

var rabbitmq = builder
    .AddRabbitMQ("Messaging",
        userName: builder.AddParameter("RabbitMQUsername", "guest", secret: true),
        password: builder.AddParameter("RabbitMQPassword", "guest", secret: true))
    .WithImage("masstransit/rabbitmq")
    .WithDataVolume("TicketBuddy.Monolith.RabbitMQ")
    .WithHttpEndpoint(port: 5672, targetPort: 5672)
    .WithHttpsEndpoint(port: 15672, targetPort: 15672);

var redis = builder
    .AddRedis("Cache")
    .WithImage("redis:7.0-alpine")
    .WithDataVolume("TicketBuddy.Monolith.Redis")
    .WithPassword(builder.AddParameter("RedisPassword", "YourStrong@Passw0rd"))
    .WithHostPort(16379)
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
    .WithEnvironment(Environment, CommonEnvironment.LocalDevelopment.ToString);

var dataseeder = builder.AddProject<Projects.Host_Dataseeder>("Dataseeder")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment(Environment, CommonEnvironment.LocalDevelopment.ToString);

builder.AddViteApp(name: "User-Interface", workingDirectory: "../../UI")
    .WithReference(api)
    .WaitFor(api)
    .WithReference(dataseeder)
    .WaitFor(dataseeder)
    .WithNpmPackageInstallation();

var app = builder.Build();
await app.RunAsync();
