using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Architecture.Events.ProjectDependencies.Rules;

internal interface ILayerRule
{
    DiagnosticDescriptor Descriptor { get; }
    bool AppliesTo(string assemblyName);
    void Validate(CompilationAnalysisContext context, string assemblyName, IReadOnlyList<string> references);
}