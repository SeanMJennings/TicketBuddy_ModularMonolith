using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies.Rules;

internal sealed class InfrastructureLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH004",
        title: "Invalid Infrastructure layer reference",
        messageFormat: "{0} cannot reference {1}. Infrastructure layer may only reference Application.Events, shared Infrastructure, Messaging.Events, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.InfrastructureEvents;
    protected override bool AllowMessagesProjects => true;
    
    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationEvents,
        KnownAssemblies.DomainEvents,
        KnownAssemblies.MessagingEvents,
        KnownAssemblies.SharedInfrastructure,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}