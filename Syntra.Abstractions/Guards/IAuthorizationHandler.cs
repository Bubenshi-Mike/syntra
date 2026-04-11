using Syntra.Abstractions.Results;

namespace Syntra.Abstractions.Guards;

/// <summary>
/// Fine-grained, request-specific authorization handler.
/// Evaluated by the authorization pipeline behavior before the request handler runs.
/// </summary>
/// <typeparam name="TRequest">
/// The request type this handler guards.
/// Must implement <see cref="IAuthorizedRequest"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// Register one or more <see cref="IAuthorizationHandler{TRequest}"/> implementations
/// for the same <typeparamref name="TRequest"/>. All registered handlers are evaluated;
/// the first failure short-circuits the chain.
/// </para>
/// <para>
/// Return <see cref="Result.Success()"/> to allow the request to proceed.
/// Return <see cref="Result.Failure(Error)"/> (typically <see cref="Error.Forbidden"/> or
/// <see cref="Error.Unauthorized"/>) to deny it:
/// <code>
/// public sealed class DeleteProductAuthorizationHandler
///     : IAuthorizationHandler&lt;DeleteProductCommand&gt;
/// {
///     public Task&lt;Result&gt; AuthorizeAsync(DeleteProductCommand request, CancellationToken ct)
///     {
///         if (!_currentUser.HasRole("Admin"))
///             return Task.FromResult(
///                 Result.Failure(Error.Forbidden("Product.Delete.Denied", "Admins only.")));
///
///         return Task.FromResult(Result.Success());
///     }
/// }
/// </code>
/// </para>
/// </remarks>
public interface IAuthorizationHandler<in TRequest>
    where TRequest : IAuthorizedRequest
{
    /// <summary>
    /// Evaluates whether the current principal is allowed to execute <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request to authorize. Never <c>null</c>.</param>
    /// <param name="cancellationToken">Token to observe for cooperative cancellation.</param>
    /// <returns>
    /// <see cref="Result.Success()"/> if authorized;
    /// a failed <see cref="Result"/> with <see cref="ErrorType.Forbidden"/> or
    /// <see cref="ErrorType.Unauthorized"/> if not.
    /// </returns>
    Task<Result> AuthorizeAsync(TRequest request, CancellationToken cancellationToken = default);
}
