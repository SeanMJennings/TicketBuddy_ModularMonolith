using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Architecture.ProjectDependencies;

public static class AssemblyReferenceReader
{
    public static IReadOnlyList<string> GetReferencedProjectAssemblies(Compilation compilation, Func<string, bool> isKnownAssembly)
    {
        return compilation.References
            .Select(GetAssemblyName)
            .Where(name => !string.IsNullOrEmpty(name) && isKnownAssembly(name))
            .ToList()!;
    }

    private static string? GetAssemblyName(MetadataReference reference)
    {
        return reference switch
        {
            CompilationReference cr => cr.Compilation.AssemblyName,
            PortableExecutableReference { Display: not null } pe => Path.GetFileNameWithoutExtension(pe.Display),
            _ => null
        };
    }
}
