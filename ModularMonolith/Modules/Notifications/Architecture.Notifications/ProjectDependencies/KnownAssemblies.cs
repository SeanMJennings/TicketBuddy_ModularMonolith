namespace Architecture.Notifications.ProjectDependencies;

internal static class KnownAssemblies
{
    internal const string DomainNotifications = "Domain.Notifications";
    internal const string ApplicationNotifications = "Application.Notifications";
    internal const string ControllersNotifications = "Controllers.Notifications";
    internal const string InfrastructureNotifications = "Infrastructure.Notifications";
    internal const string MessagesNotifications = "Messages.Notifications";
    internal const string MessagingNotifications = "Messaging.Notifications";

    internal const string SharedDomain = "Domain";
    internal const string SharedApplication = "Application";
    internal const string SharedInfrastructure = "Infrastructure";

    internal static bool IsKnownAssembly(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        return name is DomainNotifications or ApplicationNotifications or ControllersNotifications
            or InfrastructureNotifications or MessagesNotifications or MessagingNotifications
            or SharedDomain or SharedApplication or SharedInfrastructure
            || name.StartsWith("Messages.");
    }
}