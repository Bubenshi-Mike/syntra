using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Syntra.Analyzers;

/// <summary>SYN003: HandleAsync must take CancellationToken as the last parameter.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingCancellationTokenAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.HandlerMissingCancellationToken);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type)
        {
            return;
        }

        if (type.TypeKind is not (TypeKind.Class or TypeKind.Struct))
        {
            return;
        }

        var hasRequestHandler = type.AllInterfaces.Any(static i => i.OriginalDefinition.MetadataName == "IRequestHandler`2");
        var hasStreamHandler = type.AllInterfaces.Any(static i => i.OriginalDefinition.MetadataName == "IStreamQueryHandler`2");

        if (!hasRequestHandler && !hasStreamHandler)
        {
            return;
        }

        var handleAsync = SymbolHelpers.FindHandleAsync(type);
        if (handleAsync is null)
        {
            return;
        }

        if (SymbolHelpers.LastParameterIsCancellationToken(handleAsync))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.HandlerMissingCancellationToken,
                handleAsync.Locations.FirstOrDefault() ?? type.Locations.FirstOrDefault() ?? Location.None,
                type.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
    }
}
