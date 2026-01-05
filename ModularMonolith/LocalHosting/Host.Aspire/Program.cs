using TicketBuddy.AppHost.Applications;
using TicketBuddy.AppHost.Infrastructure;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgresDatabase();
var (rabbitmq, rabbitUserParam, rabbitPasswordParam) = builder.AddRabbitMqMessaging();
var redis = builder.AddRedisCache();
var keycloak = builder.AddKeycloakIdentity(rabbitUserParam, rabbitPasswordParam);

var migrations = builder.AddMigrations(database);
var api = builder.AddApi(database, migrations, rabbitmq, redis, keycloak);
var dataSeeder = builder.AddDataSeeder(api);
await builder.AddUserInterface(api, dataSeeder);

var app = builder.Build();
await app.RunAsync();