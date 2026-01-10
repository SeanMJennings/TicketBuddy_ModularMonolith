using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies.Rules;

internal sealed class MessagesLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH005",
        title: "Invalid Messages layer reference",
        messageFormat: "{0} cannot reference {1}. Messages layer may only reference Domain.Events and shared Domain.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.MessagesEvents;
    
    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.DomainEvents,
        KnownAssemblies.SharedDomain
    ];
}