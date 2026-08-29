namespace Syntra.Analyzers.Tests;

public sealed class ResultPatternAnalyzerTests
{
    [Fact]
    public async Task Throw_in_HandleAsync_returning_Task_of_Result_reports_SYN030()
    {
        const string source = """
            using System;
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record ThrowyCommand : ICommand;

            public sealed class ThrowyCommandHandler : ICommandHandler<ThrowyCommand>
            {
                public Task<Result> HandleAsync(ThrowyCommand request, CancellationToken cancellationToken = default)
                {
                    throw new InvalidOperationException("boom");
                }
            }
            """;

        var expected = AnalyzerVerifier<ResultPatternAnalyzer>
            .Diagnostic(DiagnosticDescriptors.HandlerShouldNotThrow)
            .WithSpan(14, 9, 14, 53);

        await AnalyzerVerifier<ResultPatternAnalyzer>.VerifyAsync(source, expected);
    }

    [Fact]
    public async Task Result_Failure_instead_of_throw_reports_nothing()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record SafeCommand : ICommand;

            public sealed class SafeCommandHandler : ICommandHandler<SafeCommand>
            {
                public Task<Result> HandleAsync(SafeCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Failure(Error.NotFound("SAFE_NOT_FOUND", "missing")));
            }
            """;

        await AnalyzerVerifier<ResultPatternAnalyzer>.VerifyAsync(source);
    }
}
