using Common.Environment;

namespace TicketBuddy.AppHost.Applications;

public static class ApplicationResources
{
    private const string EnvironmentVariable = "Environment";

    extension(IDistributedApplicationBuilder builder)
    {
        public IResourceBuilder<ProjectResource> AddMigrations(IResourceBuilder<PostgresDatabaseResource> database)
        {
            return builder.AddProject<Projects.Host_Migrations>("Migrations")
                .WithReference(database)
                .WaitFor(database)
                .WithEnvironment(EnvironmentVariable, CommonEnvironment.LocalDevelopment.ToString);
        }

        public IResourceBuilder<ProjectResource> AddApi(IResourceBuilder<PostgresDatabaseResource> database,
            IResourceBuilder<ProjectResource> migrations,
            IResourceBuilder<RabbitMQServerResource> rabbitmq,
            IResourceBuilder<RedisResource> redis,
            IResourceBuilder<KeycloakResource> keycloak)
        {
            return builder.AddProject<Projects.Host>("Api")
                .WithHttpEndpoint(port: 5000, name: "http")
                .WithHttpHealthCheck("/health")
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
                .WithEnvironment(EnvironmentVariable, CommonEnvironment.LocalDevelopment.ToString);
        }

        public IResourceBuilder<ProjectResource> AddDataSeeder(IResourceBuilder<ProjectResource> api)
        {
            return builder.AddProject<Projects.LocalHost_Dataseeder>("Dataseeder")
                .WithReference(api)
                .WaitFor(api)
                .WithEnvironment(EnvironmentVariable, CommonEnvironment.LocalDevelopment.ToString);
        }

        public async Task<IResourceBuilder<ContainerResource>> AddUserInterface(IResourceBuilder<ProjectResource> api,
            IResourceBuilder<ProjectResource> dataSeeder)
        {
            await UserInterface.CreateImage();

            return builder
                .AddContainer("User-Interface", UserInterface.ImageName)
                .WithHttpEndpoint(port: 5173, targetPort: 5173)
                .WithReference(api)
                .WaitFor(api)
                .WithReference(dataSeeder)
                .WaitFor(dataSeeder)
                .WithLifetime(ContainerLifetime.Persistent);
        }
    }
}

