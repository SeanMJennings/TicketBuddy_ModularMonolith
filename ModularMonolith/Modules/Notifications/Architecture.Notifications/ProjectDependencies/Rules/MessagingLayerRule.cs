using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Notifications.ProjectDependencies.Rules;

internal sealed class MessagingLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_NOTIF_006",
        title: "Invalid Messaging layer reference",
        messageFormat: "{0} cannot reference {1}. Messaging layer may only reference Application.Notifications, shared Application/Domain, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.MessagingNotifications;
    protected override bool AllowMessagesProjects => true;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationNotifications,
        KnownAssemblies.DomainNotifications,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}