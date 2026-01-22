using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Notifications.ProjectDependencies.Rules;

internal sealed class ControllersLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_NOTIF_003",
        title: "Invalid Controllers layer reference",
        messageFormat: "{0} cannot reference {1}. Controllers layer may only reference Application.Notifications and its transitive dependencies.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.ControllersNotifications;
    protected override bool AllowMessagesProjects => true;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationNotifications,
        KnownAssemblies.DomainNotifications,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}