using Syntra.Abstractions.Results;

namespace Syntra.WebAPI.Api;

public static class ResultHttpExtensions
{
    public static IResult ToHttp(this Result result) =>
        result.IsSuccess ? Results.NoContent() : result.Error!.ToHttp();

    public static IResult ToHttp<T>(this Result<T> result) =>
        result.IsSuccess ? Results.Ok(result.Value) : result.Error!.ToHttp();

    private static IResult ToHttp(this Error error) => error.Type switch
    {
        ErrorType.Validation => Results.BadRequest(new ErrorPayload(error.Code, error.Message)),
        ErrorType.NotFound => Results.NotFound(new ErrorPayload(error.Code, error.Message)),
        ErrorType.Conflict => Results.Conflict(new ErrorPayload(error.Code, error.Message)),
        ErrorType.Unauthorized => Results.Unauthorized(),
        ErrorType.Forbidden => Results.Forbid(),
        // Deliberately does not forward error.Message here - for an unexpected error, it's the
        // raw exception.Message (Error.FromException), which can carry internal details (a
        // connection string, a server-side file path, a third-party SDK's error text) that
        // shouldn't reach an external, potentially unauthenticated caller. ExceptionBehavior
        // already logs the full exception server-side before it gets here - that's where to
        // look, not the HTTP response.
        _ => Results.Problem(
            title: "An unexpected error occurred.",
            detail: error.Code,
            statusCode: StatusCodes.Status500InternalServerError),
    };

    private sealed record ErrorPayload(string Code, string Message);
}
