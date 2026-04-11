using FluentValidation.Results;

namespace Syntra.Behaviors.Validation;

/// <summary>
/// Maps FluentValidation <see cref="ValidationFailure"/> entries to <see cref="Error"/> values.
/// </summary>
public static class ValidationFailureMapper
{
    /// <summary>
    /// Converts each failure to <see cref="Error.Validation(string, string, string?)"/>.
    /// </summary>
    public static Error[] ToErrors(IEnumerable<ValidationFailure> failures) =>
        failures
            .Where(static f => f is not null)
            .Select(static f => Error.Validation(f.ErrorCode, f.ErrorMessage, f.PropertyName))
            .ToArray();
}
