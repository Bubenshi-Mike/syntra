using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Syntra.Analyzers;

/// <summary>SYN010: handler and behavior type names follow Syntra suffix conventions.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamingConventionAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.NamingConvention);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type)
            return;

        if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
            return;

        if (type.IsAbstract)
            return;

        var isRequestOrStreamHandler = type.AllInterfaces.Any(static i =>
            i.OriginalDefinition.MetadataName is "IRequestHandler`2" or "IStreamQueryHandler`2");

        if (isRequestOrStreamHandler)
        {
            if (!type.Name.EndsWith("Handler", StringComparison.Ordinal))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.NamingConvention,
                        type.Locations.FirstOrDefault() ?? Location.None,
                        $"Handler type '{type.Name}' should end with 'Handler'."));
            }
        }

        if (type.AllInterfaces.Any(static i => i.OriginalDefinition.MetadataName == "IPipelineBehavior`2"))
        {
            if (!type.Name.EndsWith("Behavior", StringComparison.Ordinal))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.NamingConvention,
                        type.Locations.FirstOrDefault() ?? Location.None,
                        $"Pipeline behavior type '{type.Name}' should end with 'Behavior'."));
            }
        }
    }
}
