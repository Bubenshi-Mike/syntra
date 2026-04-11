using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Syntra.SourceGenerator;

/// <summary>
/// Validates handler registrations and method signatures at compile time.
/// </summary>
[Generator]
public sealed class SyntraHandlerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var handlers = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                static (ctx, _) => GetHandlerModel(ctx))
            .Where(static m => m is not null)
            .Select(static (m, _) => m!)
            .Collect();

        context.RegisterSourceOutput(handlers.Combine(context.CompilationProvider), static (spc, t) => Execute(spc, t.Left, t.Right));
    }

    private static HandlerDescriptorModel? GetHandlerModel(GeneratorSyntaxContext context)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol typeSymbol)
            return null;

        INamedTypeSymbol? requestHandler = null;
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface is { IsGenericType: true } &&
                iface.OriginalDefinition.MetadataName == "IRequestHandler`2")
            {
                requestHandler = iface;
                break;
            }
        }

        if (requestHandler is null || requestHandler.TypeArguments.Length < 2)
            return null;

        if (requestHandler.TypeArguments[0] is not INamedTypeSymbol requestType)
            return null;
        var handleAsync = FindHandleAsync(typeSymbol);
        var hasCt = handleAsync is not null && HandleAsyncUsesCancellationToken(handleAsync);

        var isCmd = typeSymbol.AllInterfaces.Any(static i =>
            i is { IsGenericType: true } &&
            i.OriginalDefinition.MetadataName is "ICommandHandler`1" or "ICommandHandler`2");

        return new HandlerDescriptorModel
        {
            HandlerTypeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            RequestTypeSymbol = requestType,
            PrimaryLocation = context.Node.GetLocation(),
            HandleAsyncHasCancellationToken = hasCt,
            IsCommandHandler = isCmd,
        };
    }

    private static IMethodSymbol? FindHandleAsync(INamedTypeSymbol type)
    {
        foreach (var member in type.GetMembers("HandleAsync"))
        {
            if (member is IMethodSymbol { Parameters.Length: >= 1 } m)
                return m;
        }

        return null;
    }

    private static bool HandleAsyncUsesCancellationToken(IMethodSymbol handleAsync)
    {
        if (handleAsync.Parameters.Length < 2)
            return false;

        var last = handleAsync.Parameters[handleAsync.Parameters.Length - 1];
        return last.Type.Name == "CancellationToken";
    }

    private static void Execute(SourceProductionContext context, ImmutableArray<HandlerDescriptorModel> handlers, Compilation compilation)
    {
        var byRequest = new Dictionary<INamedTypeSymbol, List<HandlerDescriptorModel>>(SymbolEqualityComparer.Default);
        foreach (var h in handlers)
        {
            if (!byRequest.TryGetValue(h.RequestTypeSymbol, out var list))
            {
                list = [];
                byRequest[h.RequestTypeSymbol] = list;
            }

            list.Add(h);
        }

        foreach (var kvp in byRequest)
        {
            var request = kvp.Key;
            var list = kvp.Value;
            if (list.Count <= 1)
                continue;

            foreach (var h in list)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.MultipleHandlersForRequest,
                        h.PrimaryLocation,
                        request.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }

        var handledRequestsBuilder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        foreach (var key in byRequest.Keys)
            handledRequestsBuilder.Add(key);
        var handledRequests = handledRequestsBuilder.ToImmutable();

        foreach (var h in handlers)
        {
            if (!h.HandleAsyncHasCancellationToken)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.HandlerMissingCancellationToken,
                        h.PrimaryLocation,
                        h.HandlerTypeName));
            }

            if (h.IsCommandHandler && !ImplementsICommand(h.RequestTypeSymbol))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.CommandDoesNotImplementICommand,
                        h.PrimaryLocation,
                        h.RequestTypeSymbol.Name));
            }
        }

        ReportRequestsWithoutHandlers(context, compilation, handledRequests);

        HandlerRegistrationEmitter.EmitHandlerCount(context, handlers.Length);
    }

    private static bool ImplementsICommand(ITypeSymbol type)
    {
        foreach (var iface in type.AllInterfaces)
        {
            if (iface.OriginalDefinition.MetadataName is "ICommand" or "ICommand`1")
                return true;
        }

        return false;
    }

    private static void ReportRequestsWithoutHandlers(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableHashSet<INamedTypeSymbol> handledRequestTypes)
    {
        foreach (var tree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(tree);
            foreach (var node in tree.GetRoot().DescendantNodes())
            {
                if (node is not TypeDeclarationSyntax typeDecl)
                    continue;

                if (model.GetDeclaredSymbol(typeDecl) is not INamedTypeSymbol typeSymbol)
                    continue;

                if (typeSymbol.TypeKind is not (TypeKind.Class or TypeKind.Struct))
                    continue;

                if (typeSymbol.IsAbstract)
                    continue;

                if (typeSymbol.AllInterfaces.Any(static i => i.OriginalDefinition.MetadataName == "IRequestHandler`2"))
                    continue;

                if (!ImplementsIRequest(typeSymbol))
                    continue;

                if (handledRequestTypes.Contains(typeSymbol))
                    continue;

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptors.RequestHasNoHandler,
                        typeDecl.Identifier.GetLocation(),
                        typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat)));
            }
        }
    }

    private static bool ImplementsIRequest(INamedTypeSymbol typeSymbol)
    {
        foreach (var iface in typeSymbol.AllInterfaces)
        {
            if (iface is { IsGenericType: true } && iface.OriginalDefinition.MetadataName == "IRequest`1")
                return true;
        }

        return false;
    }

}
