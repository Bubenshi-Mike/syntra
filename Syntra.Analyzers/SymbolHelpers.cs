using Microsoft.CodeAnalysis;

namespace Syntra.Analyzers;

internal static class SymbolHelpers
{
    public static IEnumerable<INamedTypeSymbol> EnumerateNamedTypes(INamespaceSymbol ns)
    {
        foreach (var nestedNs in ns.GetNamespaceMembers())
        {
            foreach (var t in EnumerateNamedTypes(nestedNs))
            {
                yield return t;
            }
        }

        foreach (var type in ns.GetTypeMembers())
        {
            foreach (var t in EnumerateNestedAndSelf(type))
            {
                yield return t;
            }
        }
    }

    private static IEnumerable<INamedTypeSymbol> EnumerateNestedAndSelf(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nested in type.GetTypeMembers())
        {
            foreach (var t in EnumerateNestedAndSelf(nested))
            {
                yield return t;
            }
        }
    }

    public static bool TryGetGenericInterface(
        INamedTypeSymbol type,
        string metadataName,
        out INamedTypeSymbol? constructed)
    {
        foreach (var iface in type.AllInterfaces)
        {
            if (iface.OriginalDefinition.MetadataName == metadataName)
            {
                constructed = iface;
                return true;
            }
        }

        constructed = null;
        return false;
    }

    public static IMethodSymbol? FindHandleAsync(INamedTypeSymbol type)
    {
        foreach (var member in type.GetMembers("HandleAsync"))
        {
            if (member is IMethodSymbol { Parameters.Length: >= 1 } m)
            {
                return m;
            }
        }

        return null;
    }

    public static bool LastParameterIsCancellationToken(IMethodSymbol method)
    {
        if (method.Parameters.Length == 0)
        {
            return false;
        }

        var last = method.Parameters[method.Parameters.Length - 1];
        return last.Type.Name == "CancellationToken";
    }
}
