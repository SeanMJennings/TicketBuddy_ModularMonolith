using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Tickets.ProjectDependencies.Rules;

internal sealed class ControllersLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_TICKETS_003",
        title: "Invalid Controllers layer reference",
        messageFormat: "{0} cannot reference {1}. Controllers layer may only reference Application.Tickets and Domain.Tickets.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.ControllersTickets;
    protected override bool AllowMessagesProjects => true;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationTickets,
        KnownAssemblies.DomainTickets,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}
