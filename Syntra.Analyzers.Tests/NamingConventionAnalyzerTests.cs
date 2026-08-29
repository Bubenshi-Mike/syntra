namespace Syntra.Analyzers.Tests;

public sealed class NamingConventionAnalyzerTests
{
    [Fact]
    public async Task Handler_type_not_ending_in_Handler_reports_SYN010()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record PingCommand : ICommand;

            public sealed class PingProcessor : ICommandHandler<PingCommand>
            {
                public Task<Result> HandleAsync(PingCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }
            """;

        var expected = AnalyzerVerifier<NamingConventionAnalyzer>
            .Diagnostic(DiagnosticDescriptors.NamingConvention)
            .WithSpan(9, 21, 9, 34)
            .WithArguments("Handler type 'PingProcessor' should end with 'Handler'.");

        await AnalyzerVerifier<NamingConventionAnalyzer>.VerifyAsync(source, expected);
    }

    [Fact]
    public async Task Behavior_type_not_ending_in_Behavior_reports_SYN010()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Pipelines;
            using Syntra.Abstractions.Results;

            public sealed record PingCommand : ICommand;

            public sealed class LoggingStep : IPipelineBehavior<PingCommand, Result>
            {
                public Task<Result> HandleAsync(PingCommand request, RequestHandlerDelegate<Result> next, CancellationToken cancellationToken = default) =>
                    next(cancellationToken);
            }
            """;

        var expected = AnalyzerVerifier<NamingConventionAnalyzer>
            .Diagnostic(DiagnosticDescriptors.NamingConvention)
            .WithSpan(9, 21, 9, 32)
            .WithArguments("Pipeline behavior type 'LoggingStep' should end with 'Behavior'.");

        await AnalyzerVerifier<NamingConventionAnalyzer>.VerifyAsync(source, expected);
    }

    [Fact]
    public async Task Correctly_named_handler_and_behavior_report_nothing()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Pipelines;
            using Syntra.Abstractions.Results;

            public sealed record PingCommand : ICommand;

            public sealed class PingCommandHandler : ICommandHandler<PingCommand>
            {
                public Task<Result> HandleAsync(PingCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }

            public sealed class LoggingBehavior : IPipelineBehavior<PingCommand, Result>
            {
                public Task<Result> HandleAsync(PingCommand request, RequestHandlerDelegate<Result> next, CancellationToken cancellationToken = default) =>
                    next(cancellationToken);
            }
            """;

        await AnalyzerVerifier<NamingConventionAnalyzer>.VerifyAsync(source);
    }
}
