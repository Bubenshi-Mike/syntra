using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Syntra.Analyzers.Tests;

/// <summary>
/// Runs a source snippet through a single analyzer with Syntra.Abstractions available as a
/// reference, so fixtures can implement the real interfaces (IRequest, ICommandHandler, etc.)
/// instead of hand-rolled stand-ins.
/// </summary>
/// <remarks>
/// Uses <see cref="DefaultVerifier"/>, not the obsolete xUnit-specific verifier - the latter's
/// binary is built against an old xunit.assert and throws MissingMethodException against our
/// current xunit version. References the CURRENTLY RUNNING net10.0 runtime's assemblies directly
/// (via TRUSTED_PLATFORM_ASSEMBLIES) rather than a bundled ReferenceAssemblies.Net.NetXX catalog,
/// since this testing package predates net10.0 and its catalog's net8.0 System.Runtime is a lower
/// version than what Syntra.Abstractions.dll (built for net10.0) requires - mixing them is a CS1705
/// compile error, not just a warning.
/// </remarks>
internal static class AnalyzerVerifier<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    private static readonly ImmutableArray<MetadataReference> RuntimeReferences =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Where(static path => path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToImmutableArray();

    public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor) =>
        CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic(descriptor);

    public static async Task VerifyAsync(string source, params DiagnosticResult[] expected)
    {
        var test = new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            TestCode = source,
            ReferenceAssemblies = new ReferenceAssemblies("net10.0"),
        };

        test.TestState.AdditionalReferences.AddRange(RuntimeReferences);
        test.TestState.AdditionalReferences.Add(typeof(Abstractions.Requests.IRequest).Assembly);
        test.ExpectedDiagnostics.AddRange(expected);

        await test.RunAsync();
    }
}
