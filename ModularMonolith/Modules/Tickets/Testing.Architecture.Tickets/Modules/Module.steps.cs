using System.Reflection;
using Application.Tickets.Ticket.PurchaseTickets;
using BDD;
using Domain.Tickets.Event;

namespace Testing.Architecture.Tickets.Modules;

[TestFixture]
internal partial class ModuleSpecs : Specification
{
    private static Assembly DomainAssembly => typeof(Event).Assembly;
    private static Assembly ApplicationAssembly => typeof(PurchaseTickets).Assembly;
    private static Assembly InfrastructureAssembly => typeof(Infrastructure.Tickets.Event.EventRepository).Assembly;
    private static Assembly ControllerAssembly => typeof(Controllers.Tickets.Ticket.PurchaseTicketsEndpoint).Assembly;
    private static Assembly MessagingAssembly => typeof(Messaging.Tickets.TicketsMessaging).Assembly;
    private static Assembly MessagesAssembly => typeof(Messages.Tickets.EventSoldOut).Assembly;
    
    private string[] projectDependencies = [];
    
    protected override void before_each()
    {
        base.before_each();
        projectDependencies = [];
    }
    
    private static string[] GetProjectDependencies(Assembly assembly)
    {
        var knownProjectPrefixes = new[] { "Domain", "Application", "Infrastructure", "Controllers", "Messaging", "Messages" };
        
        return assembly.GetReferencedAssemblies()
            .Where(a => a.Name is not null && knownProjectPrefixes.Any(prefix => a.Name.StartsWith(prefix)))
            .Select(a => a.Name!)
            .ToArray();
    }

    private void checking_the_domain_project_dependencies()
    {
        projectDependencies = GetProjectDependencies(DomainAssembly);
    }

    private void checking_the_application_project_dependencies()
    {
        projectDependencies = GetProjectDependencies(ApplicationAssembly);
    }

    private void checking_the_infrastructure_project_dependencies()
    {
        projectDependencies = GetProjectDependencies(InfrastructureAssembly);
    }

    private void checking_the_controllers_project_dependencies()
    {
        projectDependencies = GetProjectDependencies(ControllerAssembly);
    }

    private void checking_the_messaging_project_dependencies()
    {
        projectDependencies = GetProjectDependencies(MessagingAssembly);
    }

    private void checking_the_messages_project_dependencies()
    {
        projectDependencies = GetProjectDependencies(MessagesAssembly);
    }

    private void it_should_only_have_dependencies_on_system_and_common_domain()
    {
        Assert.That(projectDependencies, Is.EquivalentTo(["Domain"]));
    }

    private void it_should_only_have_dependencies_on_common_application_domain_tickets_and_messages()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Domain",
            "Domain.Tickets",
            "Application",
            "Messages.Tickets",
            "Messages.Events",
            "Messages.Keycloak.Users"
        ]));
    }

    private void it_should_only_have_dependencies_on_application_and_common_infrastructure()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Application.Tickets",
            "Domain",
            "Domain.Tickets",
            "Infrastructure",
            "Messages.Events",
            "Messaging.Tickets"
        ]));
    }

    private void it_should_only_have_dependencies_on_application_and_domain()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Application.Tickets",
            "Domain.Tickets"
        ]));
    }

    private void it_should_only_have_dependencies_on_application_and_messages()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Application.Tickets",
            "Messages.Events",
            "Messages.Keycloak.Users"
        ]));
    }

    private void it_should_have_no_project_dependencies()
    {
        Assert.That(projectDependencies, Is.Empty);
    }
}