namespace Syntra.Analyzers.Tests;

public sealed class DuplicateHandlerAnalyzerTests
{
    [Fact]
    public async Task Two_handlers_for_the_same_command_reports_SYN002_on_each()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record CreateOrderCommand : ICommand;

            public sealed class CreateOrderHandlerOne : ICommandHandler<CreateOrderCommand>
            {
                public Task<Result> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }

            public sealed class CreateOrderHandlerTwo : ICommandHandler<CreateOrderCommand>
            {
                public Task<Result> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }
            """;

        var expectedOne = AnalyzerVerifier<DuplicateHandlerAnalyzer>
            .Diagnostic(DiagnosticDescriptors.DuplicateCommandHandlers)
            .WithSpan(9, 21, 9, 42)
            .WithArguments("CreateOrderCommand");
        var expectedTwo = AnalyzerVerifier<DuplicateHandlerAnalyzer>
            .Diagnostic(DiagnosticDescriptors.DuplicateCommandHandlers)
            .WithSpan(15, 21, 15, 42)
            .WithArguments("CreateOrderCommand");

        await AnalyzerVerifier<DuplicateHandlerAnalyzer>.VerifyAsync(source, expectedOne, expectedTwo);
    }

    [Fact]
    public async Task Single_handler_for_a_command_reports_nothing()
    {
        const string source = """
            using System.Threading;
            using System.Threading.Tasks;
            using Syntra.Abstractions.Requests;
            using Syntra.Abstractions.Handlers;
            using Syntra.Abstractions.Results;

            public sealed record CreateOrderCommand : ICommand;

            public sealed class CreateOrderHandler : ICommandHandler<CreateOrderCommand>
            {
                public Task<Result> HandleAsync(CreateOrderCommand request, CancellationToken cancellationToken = default) =>
                    Task.FromResult(Result.Success());
            }
            """;

        await AnalyzerVerifier<DuplicateHandlerAnalyzer>.VerifyAsync(source);
    }
}
