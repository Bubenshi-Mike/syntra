using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Syntra.Analyzers;

/// <summary>SYN002: two or more types implement ICommandHandler for the same command.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateHandlerAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DuplicateCommandHandlers);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private static void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        var compilation = context.Compilation;
        var groups = new Dictionary<(INamedTypeSymbol Cmd, ITypeSymbol Response), List<INamedTypeSymbol>>(CommandKeyComparer.Instance);

        foreach (var type in SymbolHelpers.EnumerateNamedTypes(compilation.GlobalNamespace))
        {
            if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
            {
                continue;
            }

            if (type.IsAbstract)
            {
                continue;
            }

            if (!type.AllInterfaces.Any(static i =>
                    i.OriginalDefinition.MetadataName is "ICommandHandler`1" or "ICommandHandler`2"))
            {
                continue;
            }

            foreach (var iface in type.AllInterfaces)
            {
                if (iface.OriginalDefinition.MetadataName != "IRequestHandler`2" || iface.TypeArguments.Length != 2)
                {
                    continue;
                }

                Add(groups, ((INamedTypeSymbol)iface.TypeArguments[0], iface.TypeArguments[1]), type);
                break;
            }
        }

        foreach (var entry in groups)
        {
            var list = entry.Value;
            if (list.Count <= 1)
            {
                continue;
            }

            var cmdDisplay = entry.Key.Cmd.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat);
            foreach (var handler in list)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.DuplicateCommandHandlers,
                        handler.Locations.FirstOrDefault() ?? Location.None,
                        cmdDisplay));
            }
        }
    }

    private static void Add(
        Dictionary<(INamedTypeSymbol Cmd, ITypeSymbol Response), List<INamedTypeSymbol>> groups,
        (INamedTypeSymbol Cmd, ITypeSymbol Response) key,
        INamedTypeSymbol handler)
    {
        if (!groups.TryGetValue(key, out var list))
        {
            list = new List<INamedTypeSymbol>();
            groups[key] = list;
        }

        list.Add(handler);
    }

    private sealed class CommandKeyComparer : IEqualityComparer<(INamedTypeSymbol Cmd, ITypeSymbol Response)>
    {
        public static readonly CommandKeyComparer Instance = new();

        public bool Equals((INamedTypeSymbol Cmd, ITypeSymbol Response) x, (INamedTypeSymbol Cmd, ITypeSymbol Response) y) =>
            SymbolEqualityComparer.Default.Equals(x.Cmd, y.Cmd)
            && SymbolEqualityComparer.Default.Equals(x.Response, y.Response);

        public int GetHashCode((INamedTypeSymbol Cmd, ITypeSymbol Response) obj) =>
            SymbolEqualityComparer.Default.GetHashCode(obj.Cmd) * 397
            ^ SymbolEqualityComparer.Default.GetHashCode(obj.Response);
    }
}
