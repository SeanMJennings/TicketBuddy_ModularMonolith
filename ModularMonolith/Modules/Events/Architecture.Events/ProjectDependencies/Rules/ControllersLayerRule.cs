using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies.Rules;

internal sealed class ControllersLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH003",
        title: "Invalid Controllers layer reference",
        messageFormat: "{0} cannot reference {1}. Controllers layer may only reference Application.Events and its transitive dependencies.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.ControllersEvents;
    protected override bool AllowMessagesProjects => true;
    
    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationEvents,
        KnownAssemblies.DomainEvents,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}