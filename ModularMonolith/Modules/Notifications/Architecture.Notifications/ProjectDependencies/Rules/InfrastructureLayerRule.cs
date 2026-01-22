using System.Collections.Generic;
using Architecture.ProjectDependencies;
using Microsoft.CodeAnalysis;

namespace Architecture.Notifications.ProjectDependencies.Rules;

internal sealed class InfrastructureLayerRule : LayerRuleBase
{
    public override DiagnosticDescriptor Descriptor { get; } = new(
        id: "ARCH_NOTIF_004",
        title: "Invalid Infrastructure layer reference",
        messageFormat: "{0} cannot reference {1}. Infrastructure layer may only reference Application.Notifications, shared Infrastructure, Messaging.Notifications, and Messages projects.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    protected override string TargetAssembly => KnownAssemblies.InfrastructureNotifications;
    protected override bool AllowMessagesProjects => true;

    protected override IEnumerable<string> AllowedDirectReferences =>
    [
        KnownAssemblies.ApplicationNotifications,
        KnownAssemblies.DomainNotifications,
        KnownAssemblies.MessagingNotifications,
        KnownAssemblies.SharedInfrastructure,
        KnownAssemblies.SharedApplication,
        KnownAssemblies.SharedDomain
    ];
}