using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Syntra.Analyzers;

/// <summary>SYN001: request DTO implements IRequest or IStreamQuery but no handler exists in the compilation.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HandlerMissingAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.RequestHasNoHandler);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private static void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        var compilation = context.Compilation;
        var allTypes = SymbolHelpers.EnumerateNamedTypes(compilation.GlobalNamespace).ToList();

        var requestHandlers = new HashSet<(INamedTypeSymbol Request, ITypeSymbol Response)>(RequestResponseComparer.Instance);
        var streamHandlers = new HashSet<(INamedTypeSymbol Query, ITypeSymbol Item)>(StreamComparer.Instance);

        foreach (var type in allTypes)
        {
            if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
                continue;
            if (type.IsAbstract)
                continue;

            foreach (var iface in type.AllInterfaces)
            {
                if (iface.OriginalDefinition.MetadataName == "IRequestHandler`2" && iface.TypeArguments.Length == 2)
                {
                    requestHandlers.Add(((INamedTypeSymbol)iface.TypeArguments[0], iface.TypeArguments[1]));
                }
                else if (iface.OriginalDefinition.MetadataName == "IStreamQueryHandler`2" && iface.TypeArguments.Length == 2)
                {
                    streamHandlers.Add(((INamedTypeSymbol)iface.TypeArguments[0], iface.TypeArguments[1]));
                }
            }
        }

        foreach (var type in allTypes)
        {
            if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
                continue;
            if (type.IsAbstract)
                continue;

            if (type.AllInterfaces.Any(static i => i.OriginalDefinition.MetadataName == "IRequestHandler`2"))
                continue;

            if (TryGetGenericIRequest(type, out var response))
            {
                if (!requestHandlers.Contains((type, response!)))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.RequestHasNoHandler,
                            type.Locations.FirstOrDefault() ?? Location.None,
                            type.Name));
                }

                continue;
            }

            if (TryGetIStreamQuery(type, out var itemType))
            {
                if (!streamHandlers.Contains((type, itemType!)))
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.RequestHasNoHandler,
                            type.Locations.FirstOrDefault() ?? Location.None,
                            type.Name));
                }
            }
        }
    }

    private static bool TryGetGenericIRequest(INamedTypeSymbol type, out ITypeSymbol? response)
    {
        foreach (var iface in type.AllInterfaces)
        {
            if (iface.OriginalDefinition.MetadataName == "IRequest`1" && iface.TypeArguments.Length == 1)
            {
                response = iface.TypeArguments[0];
                return true;
            }
        }

        response = null;
        return false;
    }

    private static bool TryGetIStreamQuery(INamedTypeSymbol type, out ITypeSymbol? itemType)
    {
        foreach (var iface in type.AllInterfaces)
        {
            if (iface.OriginalDefinition.MetadataName == "IStreamQuery`1" && iface.TypeArguments.Length == 1)
            {
                itemType = iface.TypeArguments[0];
                return true;
            }
        }

        itemType = null;
        return false;
    }

    private sealed class RequestResponseComparer : IEqualityComparer<(INamedTypeSymbol Request, ITypeSymbol Response)>
    {
        public static readonly RequestResponseComparer Instance = new();

        public bool Equals((INamedTypeSymbol Request, ITypeSymbol Response) x, (INamedTypeSymbol Request, ITypeSymbol Response) y) =>
            SymbolEqualityComparer.Default.Equals(x.Request, y.Request)
            && SymbolEqualityComparer.Default.Equals(x.Response, y.Response);

        public int GetHashCode((INamedTypeSymbol Request, ITypeSymbol Response) obj) =>
            SymbolEqualityComparer.Default.GetHashCode(obj.Request) * 397
            ^ SymbolEqualityComparer.Default.GetHashCode(obj.Response);
    }

    private sealed class StreamComparer : IEqualityComparer<(INamedTypeSymbol Query, ITypeSymbol Item)>
    {
        public static readonly StreamComparer Instance = new();

        public bool Equals((INamedTypeSymbol Query, ITypeSymbol Item) x, (INamedTypeSymbol Query, ITypeSymbol Item) y) =>
            SymbolEqualityComparer.Default.Equals(x.Query, y.Query)
            && SymbolEqualityComparer.Default.Equals(x.Item, y.Item);

        public int GetHashCode((INamedTypeSymbol Query, ITypeSymbol Item) obj) =>
            SymbolEqualityComparer.Default.GetHashCode(obj.Query) * 397
            ^ SymbolEqualityComparer.Default.GetHashCode(obj.Item);
    }
}
