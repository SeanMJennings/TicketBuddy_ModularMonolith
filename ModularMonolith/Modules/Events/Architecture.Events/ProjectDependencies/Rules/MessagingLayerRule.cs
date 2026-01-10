using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies.Rules;

internal sealed class MessagingLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH006",
        title: "Invalid Messaging layer reference",
        messageFormat: "{0} cannot reference {1}. Messaging layer may only reference Application.Events, shared Application/Domain, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.MessagingEvents;
    protected override bool AllowMessagesProjects => true;
    
    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationEvents,
        KnownAssemblies.DomainEvents,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}