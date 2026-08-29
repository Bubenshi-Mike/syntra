namespace Syntra.Analyzers.Tests;

public sealed class PipelineBehaviorOrderAnalyzerTests
{
    // The analyzer only pattern-matches on method names ("AddSyntra", "ScanAssemblies",
    // "AddBehaviors"), so a minimal fake fluent API is enough - no need to reference the
    // real Syntra.DependencyInjection builder.
    private const string FakeApi = """
        using System;

        public sealed class FakeConfig
        {
            public FakeConfig AddBehaviors(Action<FakeConfig> configure) { configure(this); return this; }
            public FakeConfig ScanAssemblies(params Type[] assemblies) => this;
        }

        public static class FakeServiceCollectionExtensions
        {
            public static void AddSyntra(this object services, object configuration, Action<FakeConfig> configure) { }
        }
        """;

    // Block-bodied configure lambda: every call is a genuine descendant of lambda.Body, so both
    // ScanAssemblies and AddBehaviors are visited by the analyzer's DescendantNodes() walk.
    [Fact]
    public async Task ScanAssemblies_before_AddBehaviors_in_a_block_lambda_reports_SYN020()
    {
        var source = FakeApi + """

            public class Usage
            {
                public void Configure(object services, object configuration)
                {
                    services.AddSyntra(configuration, c =>
                    {
                        c.ScanAssemblies(typeof(Usage));
                        c.AddBehaviors(b => { });
                    });
                }
            }
            """;

        var expected = AnalyzerVerifier<PipelineBehaviorOrderAnalyzer>
            .Diagnostic(DiagnosticDescriptors.BehaviorRegistrationOrder)
            .WithSpan(17, 9, 21, 11);

        await AnalyzerVerifier<PipelineBehaviorOrderAnalyzer>.VerifyAsync(source, expected);
    }

    [Fact]
    public async Task AddBehaviors_before_ScanAssemblies_in_a_block_lambda_reports_nothing()
    {
        var source = FakeApi + """

            public class Usage
            {
                public void Configure(object services, object configuration)
                {
                    services.AddSyntra(configuration, c =>
                    {
                        c.AddBehaviors(b => { });
                        c.ScanAssemblies(typeof(Usage));
                    });
                }
            }
            """;

        await AnalyzerVerifier<PipelineBehaviorOrderAnalyzer>.VerifyAsync(source);
    }

    // Known limitation, not a test-harness bug: AnalyzeInvocation walks lambda.Body.DescendantNodes(),
    // which excludes lambda.Body itself. For a single expression-bodied chain "c => c.X().Y()", the
    // outermost/last call in the chain IS lambda.Body and is therefore never visited - only calls
    // nested inside its receiver are. Since ScanAssemblies is last in the chain, scanMin never gets
    // set and the analyzer silently no-ops, even though the ordering is exactly what SYN020 exists to
    // catch. This is also the README's documented usage shape
    // (`c => c.AddBehaviors(...).ScanAssemblies(...)`), so SYN020 likely never fires in practice.
    [Fact]
    public async Task Wrong_order_as_a_single_expression_chain_silently_reports_nothing()
    {
        var source = FakeApi + """

            public class Usage
            {
                public void Configure(object services, object configuration)
                {
                    services.AddSyntra(configuration, c => c.ScanAssemblies(typeof(Usage)).AddBehaviors(b => { }));
                }
            }
            """;

        await AnalyzerVerifier<PipelineBehaviorOrderAnalyzer>.VerifyAsync(source);
    }
}
