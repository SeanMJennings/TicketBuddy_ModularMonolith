using System.Reflection;
using Application.Events;
using BDD;
using Domain.Events;

namespace Testing.Architecture.Events.Modules;

[TestFixture]
internal partial class ModuleSpecs : Specification
{
    private static Assembly DomainAssembly => typeof(Event).Assembly;
    private static Assembly ApplicationAssembly => typeof(CreateEvent).Assembly;
    private static Assembly InfrastructureAssembly => typeof(Infrastructure.Events.Event.EventRepository).Assembly;
    private static Assembly ControllerAssembly => typeof(Controllers.Events.GetEventsEndpoint).Assembly;
    private static Assembly MessagingAssembly => typeof(Messaging.Events.EventsMessaging).Assembly;
    private static Assembly MessagesAssembly => typeof(Messages.Events.EventUpserted).Assembly;
    
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

    private void it_should_only_have_dependencies_on_domain_and_messages()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Domain",
            "Domain.Events",
            "Messages.Tickets"
        ]));
    }

    private void it_should_only_have_dependencies_on_application_and_common_infrastructure()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Application",
            "Application.Events",
            "Domain",
            "Domain.Events",
            "Infrastructure",
            "Messages.Events",
            "Messages.Tickets",
            "Messaging.Events"
        ]));
    }

    private void it_should_only_have_dependencies_on_application_and_domain()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Application.Events",
            "Domain.Events",
            "Domain"
        ]));
    }

    private void it_should_only_have_dependencies_on_application_and_messages()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Application.Events",
            "Messages.Tickets"
        ]));
    }

    private void it_should_only_have_dependencies_on_domain()
    {
        Assert.That(projectDependencies, Is.EquivalentTo([
            "Domain"
        ]));
    }
}