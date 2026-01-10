using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Tickets.ProjectDependencies.Rules;

internal sealed class ApplicationLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_TICKETS_002",
        title: "Invalid Application layer reference",
        messageFormat: "{0} cannot reference {1}. Application layer may only reference Domain.Tickets, shared Application/Domain, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.ApplicationTickets;
    protected override bool AllowMessagesProjects => true;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.DomainTickets,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}
