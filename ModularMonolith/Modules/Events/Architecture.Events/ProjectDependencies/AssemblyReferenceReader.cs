using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Architecture.Events.ProjectDependencies;

internal static class AssemblyReferenceReader
{
    internal static IReadOnlyList<string> GetReferencedProjectAssemblies(Compilation compilation)
    {
        return compilation.References
            .Select(GetAssemblyName)
            .Where(name => !string.IsNullOrEmpty(name) && KnownAssemblies.IsKnownAssembly(name))
            .ToList()!;
    }

    private static string? GetAssemblyName(MetadataReference reference)
    {
        return reference switch
        {
            CompilationReference cr => cr.Compilation.AssemblyName,
            PortableExecutableReference pe when pe.Display is not null => Path.GetFileNameWithoutExtension(pe.Display),
            _ => null
        };
    }
}