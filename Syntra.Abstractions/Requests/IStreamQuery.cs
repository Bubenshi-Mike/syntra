namespace Syntra.Abstractions.Requests;

/// <summary>
/// Marker for a streaming query whose results are pushed incrementally
/// as an <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type of each item in the stream.
/// The mediator surfaces items as <c>IAsyncEnumerable&lt;TResponse&gt;</c>.
/// </typeparam>
/// <remarks>
/// <para>
/// <see cref="IStreamQuery{TResponse}"/> is intentionally NOT derived from
/// <see cref="IRequest{TResponse}"/>. Streaming requests follow a separate
/// dispatch path through
/// <c>ISyntraMediator.CreateStreamAsync&lt;TResponse&gt;</c>, not
/// <c>ISyntraMediator.SendAsync&lt;TResponse&gt;</c>.
/// </para>
/// <para>
/// Handlers implement <c>IStreamQueryHandler&lt;TQuery, TResponse&gt;</c>
/// and return <c>IAsyncEnumerable&lt;TResponse&gt;</c> directly.
/// Wrap items in <c>Result&lt;TResponse&gt;</c> to propagate per-item errors.
/// </para>
/// </remarks>
public interface IStreamQuery<out TResponse> : IRequest { }
