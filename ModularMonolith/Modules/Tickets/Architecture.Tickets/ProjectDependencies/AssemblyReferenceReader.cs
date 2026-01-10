using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using SharedReader = Architecture.ProjectDependencies.AssemblyReferenceReader;

namespace Architecture.Tickets.ProjectDependencies;

internal static class AssemblyReferenceReader
{
    internal static IReadOnlyList<string> GetReferencedProjectAssemblies(Compilation compilation)
    {
        return SharedReader.GetReferencedProjectAssemblies(compilation, KnownAssemblies.IsKnownAssembly);
    }
}
