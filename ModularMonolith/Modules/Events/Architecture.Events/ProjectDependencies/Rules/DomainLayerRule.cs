using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies.Rules;

internal sealed class DomainLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH001",
        title: "Invalid Domain layer reference",
        messageFormat: "{0} cannot reference {1}. Domain layer may only reference shared Domain.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.DomainEvents;
    
    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.SharedDomain
    ];
}