using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies.Rules;

internal sealed class ApplicationLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH002",
        title: "Invalid Application layer reference",
        messageFormat: "{0} cannot reference {1}. Application layer may only reference Domain.Events, shared Application/Domain, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.ApplicationEvents;
    protected override bool AllowMessagesProjects => true;
    
    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.DomainEvents,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}