namespace Syntra.Behaviors.Authorization;

/// <summary>
/// Options for <see cref="AuthorizationBehavior{TRequest,TResponse}"/> (ASP.NET Core policy checks).
/// </summary>
public sealed class AuthorizationBehaviorOptions
{
    /// <summary>
    /// When no <c>HttpContext</c> is available, fail closed (<c>true</c>) or allow the request (<c>false</c>).
    /// Default: <c>true</c>.
    /// </summary>
    public bool FailWhenHttpContextMissing { get; set; } = true;
}
