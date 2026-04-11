using FluentValidation;
using Syntra.Behaviors.Shared;

namespace Syntra.Behaviors.Validation;

/// <summary>
/// Runs all <see cref="IValidator{T}"/> registrations for <typeparamref name="TRequest"/> in parallel,
/// then aggregates failures into a single failed <see cref="Result"/> / <see cref="Result{T}"/>.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        var validatorArray = validators as IValidator<TRequest>[] ?? validators.ToArray();

        if (validatorArray.Length == 0)
            return await next(cancellationToken).ConfigureAwait(false);

        logger.LogDebug(
            "Running {ValidatorCount} validator(s) for {RequestName}",
            validatorArray.Length,
            typeof(TRequest).Name);

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
                validatorArray.Select(v => v.ValidateAsync(context, cancellationToken)))
            .ConfigureAwait(false);

        var failures = validationResults
            .SelectMany(static r => r.Errors)
            .ToArray();

        if (failures.Length == 0)
            return await next(cancellationToken).ConfigureAwait(false);

        logger.LogDebug(
            "Validation failed for {RequestName} with {FailureCount} error(s)",
            typeof(TRequest).Name,
            failures.Length);

        var errors = ValidationFailureMapper.ToErrors(failures);
        return (TResponse)ResultFailureMapper.CreateFailureResult(typeof(TResponse), errors);
    }
}
