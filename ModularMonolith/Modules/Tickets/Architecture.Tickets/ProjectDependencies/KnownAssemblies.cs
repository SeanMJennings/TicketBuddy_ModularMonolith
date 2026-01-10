namespace Architecture.Tickets.ProjectDependencies;

internal static class KnownAssemblies
{
    internal const string DomainTickets = "Domain.Tickets";
    internal const string ApplicationTickets = "Application.Tickets";
    internal const string ControllersTickets = "Controllers.Tickets";
    internal const string InfrastructureTickets = "Infrastructure.Tickets";
    internal const string MessagesTickets = "Messages.Tickets";
    internal const string MessagingTickets = "Messaging.Tickets";

    internal const string SharedDomain = "Domain";
    internal const string SharedApplication = "Application";
    internal const string SharedInfrastructure = "Infrastructure";

    internal static bool IsKnownAssembly(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        return name is DomainTickets or ApplicationTickets or ControllersTickets
            or InfrastructureTickets or MessagesTickets or MessagingTickets
            or SharedDomain or SharedApplication or SharedInfrastructure
            || name.StartsWith("Messages.");
    }
}
