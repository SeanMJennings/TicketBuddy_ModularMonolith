using System.Collections.Immutable;
using System.Linq;
using Architecture.Events.ProjectDependencies.Rules;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Architecture.Events.ProjectDependencies;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProjectDependenciesGuardian : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<ILayerRule> LayerRules =
    [
        new DomainLayerRule(),
        new ApplicationLayerRule(),
        new ControllersLayerRule(),
        new InfrastructureLayerRule(),
        new MessagesLayerRule(),
        new MessagingLayerRule()
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [..LayerRules.Select(r => r.Descriptor)];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(Analyze);
    }

    private static void Analyze(CompilationAnalysisContext context)
    {
        var currentAssembly = context.Compilation.AssemblyName;
        if (string.IsNullOrEmpty(currentAssembly)) return;

        var referencedAssemblies = AssemblyReferenceReader.GetReferencedProjectAssemblies(context.Compilation);

        foreach (var rule in LayerRules.Where(r => r.AppliesTo(currentAssembly)))
        {
            rule.Validate(context, currentAssembly, referencedAssemblies);
        }
    }
}