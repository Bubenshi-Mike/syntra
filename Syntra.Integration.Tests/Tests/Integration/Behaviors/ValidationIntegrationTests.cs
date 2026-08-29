using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Syntra.Abstractions.Handlers;
using Syntra.Abstractions.Mediator;
using Syntra.Abstractions.Requests;
using Syntra.Abstractions.Results;
using Syntra.DependencyInjection.Registration;

namespace Syntra.Integration.Tests.Tests.Integration.Behaviors;

public sealed record ValidatedQuery(int X = 0) : IQuery<int>;

public sealed class ValidatedQueryValidator : AbstractValidator<ValidatedQuery>
{
    public ValidatedQueryValidator()
    {
        RuleFor(q => q.X).Equal(1);
    }
}

public sealed class ValidatedQueryHandler : IQueryHandler<ValidatedQuery, int>
{
    public Task<Result<int>> HandleAsync(ValidatedQuery request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success(42));
}

public sealed class ValidationIntegrationTests
{
    [Fact]
    public async Task Validation_failure_short_circuits_before_handler()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSyntra(c => c
            .AddBehaviors(b => b.AddValidation())
            .ScanAssemblies(typeof(ValidatedQuery).Assembly)
            .ValidateDependencies(false));

        await using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<ISyntraMediator>();

        var result = await mediator.SendAsync(new ValidatedQuery(0));

        Assert.True(result.IsFailure);
    }
}
