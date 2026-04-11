using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Syntra.Analyzers;

/// <summary>SYN020: AddBehaviors should appear before ScanAssemblies in AddSyntra configuration.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PipelineBehaviorOrderAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.BehaviorRegistrationOrder);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
            return;

        var semanticModel = context.SemanticModel;
        if (semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol method)
            return;

        if (method.Name != "AddSyntra")
            return;

        if (method.Parameters.Length < 2)
            return;

        var configureArg = invocation.ArgumentList.Arguments[method.Parameters.Length - 1];
        if (configureArg.Expression is not AnonymousFunctionExpressionSyntax lambda)
            return;

        if (lambda.Body is null)
            return;

        int? scanMin = null;
        int? addBehaviorsMin = null;

        foreach (var node in lambda.Body.DescendantNodes())
        {
            if (node is not InvocationExpressionSyntax inner)
                continue;

            if (semanticModel.GetSymbolInfo(inner).Symbol is not IMethodSymbol innerMethod)
                continue;

            if (innerMethod.Name == "ScanAssemblies")
            {
                var pos = inner.GetLocation().SourceSpan.Start;
                scanMin = scanMin is null ? pos : Math.Min(scanMin.Value, pos);
            }
            else if (innerMethod.Name == "AddBehaviors")
            {
                var pos = inner.GetLocation().SourceSpan.Start;
                addBehaviorsMin = addBehaviorsMin is null ? pos : Math.Min(addBehaviorsMin.Value, pos);
            }
        }

        if (scanMin is null || addBehaviorsMin is null)
            return;

        if (scanMin.Value < addBehaviorsMin.Value)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.BehaviorRegistrationOrder,
                    invocation.GetLocation()));
        }
    }
}
