using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Syntra.Analyzers;

/// <summary>SYN030: handlers returning Result should prefer Result.Failure over throw for domain failures.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ResultPatternAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.HandlerShouldNotThrow);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterOperationAction(AnalyzeThrow, OperationKind.Throw);
    }

    private static void AnalyzeThrow(OperationAnalysisContext context)
    {
        if (context.Operation is not IThrowOperation throwOp)
        {
            return;
        }

        if (context.ContainingSymbol is not IMethodSymbol method)
        {
            return;
        }

        if (method.Name != "HandleAsync")
        {
            return;
        }

        if (method.ContainingType is not INamedTypeSymbol containing)
        {
            return;
        }

        if (!containing.AllInterfaces.Any(static i => i.OriginalDefinition.MetadataName == "IRequestHandler`2"))
        {
            return;
        }

        if (!ReturnsTaskOfResult(method, context.Compilation))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                DiagnosticDescriptors.HandlerShouldNotThrow,
                throwOp.Syntax.GetLocation()));
    }

    private static bool ReturnsTaskOfResult(IMethodSymbol method, Compilation compilation)
    {
        if (method.ReturnType is not INamedTypeSymbol ret)
        {
            return false;
        }

        if (ret.OriginalDefinition.MetadataName != "Task`1" || ret.TypeArguments.Length != 1)
        {
            return false;
        }

        var inner = ret.TypeArguments[0];
        var result = compilation.GetTypeByMetadataName("Syntra.Abstractions.Results.Result");
        var resultT = compilation.GetTypeByMetadataName("Syntra.Abstractions.Results.Result`1");

        if (result is not null && SymbolEqualityComparer.Default.Equals(inner, result))
        {
            return true;
        }

        if (inner.OriginalDefinition.MetadataName == "Result`1" && resultT is not null
            && SymbolEqualityComparer.Default.Equals(inner.OriginalDefinition, resultT))
        {
            return true;
        }

        return false;
    }
}
