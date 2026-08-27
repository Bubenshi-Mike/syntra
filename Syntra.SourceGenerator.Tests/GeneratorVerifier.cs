using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Syntra.SourceGenerator.Tests;

/// <summary>
/// Runs a source snippet through <see cref="SyntraHandlerGenerator"/> with Syntra.Abstractions
/// available as a reference, so fixtures can implement the real interfaces (IRequest,
/// ICommandHandler, etc.) instead of hand-rolled stand-ins.
/// </summary>
/// <remarks>
/// Same infrastructure choices as Syntra.Analyzers.Tests' AnalyzerVerifier and for the same
/// reasons: DefaultVerifier (not the binary-incompatible XUnitVerifier) and the currently
/// running net10.0 runtime's assemblies (not the testing package's pre-net10.0
/// ReferenceAssemblies.Net.NetXX catalog, which conflicts with Syntra.Abstractions.dll's
/// actual System.Runtime version).
/// </remarks>
internal static class GeneratorVerifier
{
    private static readonly ImmutableArray<MetadataReference> RuntimeReferences =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Where(static path => path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToImmutableArray();

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) =>
        new(descriptor);

    public static async Task VerifyAsync(
        string source,
        (string HintName, string Content)? generatedHint = null,
        params DiagnosticResult[] expectedDiagnostics)
    {
        var test = new CSharpSourceGeneratorTest<SyntraHandlerGenerator, DefaultVerifier>
        {
            TestState =
            {
                Sources = { source },
                ReferenceAssemblies = new ReferenceAssemblies("net10.0"),
            },
        };

        test.TestState.AdditionalReferences.AddRange(RuntimeReferences);
        test.TestState.AdditionalReferences.Add(typeof(Abstractions.Requests.IRequest).Assembly);
        test.ExpectedDiagnostics.AddRange(expectedDiagnostics);

        if (generatedHint is { } hint)
        {
            test.TestState.GeneratedSources.Add(
                (typeof(SyntraHandlerGenerator), hint.HintName, hint.Content));
        }

        await test.RunAsync();
    }
}
