namespace Syntra.Analyzers.Tests;

public sealed class MissingCancellationTokenAnalyzerTests
{
    // The interface requires an exact-signature HandleAsync(TRequest, CancellationToken) overload to
    // implicitly implement ICommandHandler<>, so a compiling "missing token" fixture needs a second,
    // non-conforming HandleAsync overload for SymbolHelpers.FindHandleAsync to pick up instead.
    [Fact]
    public async Task Handler_with_a_non_conforming_HandleAsync_overload_reports_SYN003()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record PingCommand : ICommand;

            public sealed class PingCommandHandler : ICommandHandler<PingCommand>
            {
                public Task<Result> HandleAsync(PingCommand request) =>
                    HandleAsync(request, CancellationToken.None);

                public Task<Result> HandleAsync(PingCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }
            """;

        var expected = AnalyzerVerifier<MissingCancellationTokenAnalyzer>
            .Diagnostic(DiagnosticDescriptors.HandlerMissingCancellationToken)
            .WithSpan(11, 25, 11, 36)
            .WithArguments("PingCommandHandler");

        await AnalyzerVerifier<MissingCancellationTokenAnalyzer>.VerifyAsync(source, expected);
    }

    [Fact]
    public async Task Handler_with_only_the_conforming_HandleAsync_reports_nothing()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record PongCommand : ICommand;

            public sealed class PongCommandHandler : ICommandHandler<PongCommand>
            {
                public Task<Result> HandleAsync(PongCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }
            """;

        await AnalyzerVerifier<MissingCancellationTokenAnalyzer>.VerifyAsync(source);
    }
}
