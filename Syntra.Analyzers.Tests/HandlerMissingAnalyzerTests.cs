namespace Syntra.Analyzers.Tests;

public sealed class HandlerMissingAnalyzerTests
{
    [Fact]
    public async Task Command_without_any_handler_reports_SYN001()
    {
        const string source = """
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Results;
            using System.Threading;
            using System.Threading.Tasks;

            public sealed record LonelyCommand : ICommand;
            """;

        var expected = AnalyzerVerifier<HandlerMissingAnalyzer>
            .Diagnostic(DiagnosticDescriptors.RequestHasNoHandler)
            .WithSpan(6, 22, 6, 35)
            .WithArguments("LonelyCommand");

        await AnalyzerVerifier<HandlerMissingAnalyzer>.VerifyAsync(source, expected);
    }

    [Fact]
    public async Task Command_with_a_matching_handler_reports_nothing()
    {
        const string source = """
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;
            using System.Threading;
            using System.Threading.Tasks;

            public sealed record HandledCommand : ICommand;

            public sealed class HandledCommandHandler : ICommandHandler<HandledCommand>
            {
                public Task<Result> HandleAsync(HandledCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }
            """;

        await AnalyzerVerifier<HandlerMissingAnalyzer>.VerifyAsync(source);
    }
}
