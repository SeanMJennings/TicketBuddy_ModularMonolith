namespace Architecture.Events.ProjectDependencies;

internal static class KnownAssemblies
{
    internal const string DomainEvents = "Domain.Events";
    internal const string ApplicationEvents = "Application.Events";
    internal const string ControllersEvents = "Controllers.Events";
    internal const string InfrastructureEvents = "Infrastructure.Events";
    internal const string MessagesEvents = "Messages.Events";
    internal const string MessagingEvents = "Messaging.Events";

    internal const string SharedDomain = "Domain";
    internal const string SharedApplication = "Application";
    internal const string SharedInfrastructure = "Infrastructure";

    internal static bool IsKnownAssembly(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        return name is DomainEvents or ApplicationEvents or ControllersEvents
            or InfrastructureEvents or MessagesEvents or MessagingEvents
            or SharedDomain or SharedApplication or SharedInfrastructure
            || name.StartsWith("Messages.");
    }
}