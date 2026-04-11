using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Syntra.Abstractions.Guards;
using Syntra.Behaviors.Shared;

namespace Syntra.Behaviors.Authorization;

/// <summary>
/// Enforces ASP.NET Core authorization policies for requests implementing <see cref="IAuthorizedRequest"/>.
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse>(
    IHttpContextAccessor httpContextAccessor,
    IAuthorizationService authorizationService,
    AuthorizationBehaviorOptions options,
    ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).Name;

    /// <inheritdoc />
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken = default)
    {
        if (request is not IAuthorizedRequest authorizedRequest)
            return await next(cancellationToken).ConfigureAwait(false);

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            logger.LogWarning("No HttpContext for authorized request {RequestName}", RequestName);
            if (options.FailWhenHttpContextMissing)
            {
                var err = Error.Unauthorized("Auth.NoContext", "No HTTP context is available for authorization.");
                return ResultFailureMapper.CreateFailureResult<TResponse>(err);
            }

            return await next(cancellationToken).ConfigureAwait(false);
        }

        var user = httpContext.User;
        var policy = authorizedRequest.RequiredPolicy;

        AuthorizationResult authResult;
        if (string.IsNullOrEmpty(policy))
        {
            authResult = await authorizationService
                .AuthorizeAsync(user, resource: null, requirement: new DenyAnonymousAuthorizationRequirement())
                .ConfigureAwait(false);
        }
        else
        {
            authResult = await authorizationService
                .AuthorizeAsync(user, resource: null, policyName: policy)
                .ConfigureAwait(false);
        }

        if (!authResult.Succeeded)
        {
            logger.LogWarning(
                "Authorization denied for {RequestName} (policy: {Policy})",
                RequestName,
                policy ?? "(authenticated)");

            var err = string.IsNullOrEmpty(policy)
                ? Error.Unauthorized("Auth.NotAuthenticated", "The current user is not authenticated.")
                : Error.Forbidden("Auth.PolicyDenied", $"Policy '{policy}' was not satisfied.");

            return ResultFailureMapper.CreateFailureResult<TResponse>(err);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
