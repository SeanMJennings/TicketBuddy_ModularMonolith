using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Tickets.ProjectDependencies.Rules;

internal sealed class InfrastructureLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_TICKETS_004",
        title: "Invalid Infrastructure layer reference",
        messageFormat: "{0} cannot reference {1}. Infrastructure layer may only reference Application.Tickets, shared Infrastructure/Domain, Messaging.Tickets, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.InfrastructureTickets;
    protected override bool AllowMessagesProjects => true;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationTickets,
        KnownAssemblies.DomainTickets,
        KnownAssemblies.MessagingTickets,
        KnownAssemblies.SharedInfrastructure,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}
