namespace Syntra.Behaviors.Retry;

/// <summary>
/// Marker: <see cref="RetryBehavior{TRequest,TResponse}"/> should apply Polly retry to this request type.
/// </summary>
public interface IRetryableRequest;
