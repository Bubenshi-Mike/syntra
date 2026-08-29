namespace Syntra.Abstractions.Guards;

/// <summary>
/// Opt-in contract for requests that require authorization before the handler executes.
/// In ASP.NET Core hosts, <c>Syntra.Behaviors.Authorization.AuthorizationBehavior</c> can enforce
/// <see cref="RequiredPolicy"/> via <c>IAuthorizationService</c>. You may still use
/// <see cref="IAuthorizationHandler{TRequest}"/> from domain code with a custom pipeline hook.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="IAuthorizedRequest"/> is intentionally decoupled from ASP.NET Core's
/// authorization model so it can be used in any host (console, Worker Service, etc.).
/// </para>
/// <para>
/// Example:
/// <code>
/// public sealed record DeleteProductCommand(Guid Id)
///     : ICommand, IAuthorizedRequest
/// {
///     public string? RequiredPolicy =&gt; "Products.Delete";
/// }
/// </code>
/// </para>
/// </remarks>
public interface IAuthorizedRequest : IRequest
{
    /// <summary>
    /// The name of the policy that the current user must satisfy.
    /// <c>null</c> means any authenticated user is allowed (authentication only, no policy check).
    /// </summary>
    public string? RequiredPolicy => null;
}
