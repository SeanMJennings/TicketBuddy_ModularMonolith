using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Notifications.ProjectDependencies.Rules;

internal sealed class MessagesLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_NOTIF_005",
        title: "Invalid Messages layer reference",
        messageFormat: "{0} cannot reference {1}. Messages layer may only reference Domain.Notifications and shared Domain.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.MessagesNotifications;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.DomainNotifications,
        KnownAssemblies.SharedDomain
    ];
}