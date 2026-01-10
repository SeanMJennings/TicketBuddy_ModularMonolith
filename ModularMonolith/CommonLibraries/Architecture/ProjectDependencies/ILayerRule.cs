using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Architecture.ProjectDependencies;

public interface ILayerRule
{
    DiagnosticDescriptor Descriptor { get; }
    bool AppliesTo(string assemblyName);
    void Validate(CompilationAnalysisContext context, string assemblyName, IReadOnlyList<string> references);
}
