namespace TicketBuddy.AppHost.Infrastructure;

public static class ContainerExtensions
{
    public const string ProjectName = "Ticketbuddy.Aspire";

    public static IResourceBuilder<T> WithComposeProjectLabel<T>(this IResourceBuilder<T> builder)
        where T : ContainerResource
        => builder.WithContainerRuntimeArgs("--label", $"com.docker.compose.project={ProjectName}");
}