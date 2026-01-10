using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Architecture.Events.ProjectDependencies.Rules;

internal abstract class LayerRuleBase : ILayerRule
{
    public abstract DiagnosticDescriptor Descriptor { get; }
    
    protected abstract string TargetAssembly { get; }
    protected abstract IEnumerable<string> AllowedDirectReferences { get; }
    protected virtual bool AllowMessagesProjects => false;

    public bool AppliesTo(string assemblyName) => assemblyName == TargetAssembly;

    public void Validate(CompilationAnalysisContext context, string assemblyName, IReadOnlyList<string> references)
    {
        var allowed = AllowedDirectReferences.ToHashSet();
        
        foreach (var reference in references)
        {
            if (allowed.Contains(reference)) continue;
            if (AllowMessagesProjects && reference.StartsWith("Messages.")) continue;
            
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, Location.None, assemblyName, reference));
        }
    }
}