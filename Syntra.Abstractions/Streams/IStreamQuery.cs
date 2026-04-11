using Syntra.Abstractions.Requests;

namespace Syntra.Abstractions.Streams;

/// <summary>
/// Streaming query marker, accessible from the <c>Syntra.Syntra.Abstractions.Streams</c> namespace.
/// </summary>
/// <typeparam name="TResponse">The type of each item in the stream.</typeparam>
/// <remarks>
/// <para>
/// This interface extends <see cref="Requests.IStreamQuery{TResponse}"/>, which is the
/// canonical definition used by the mediator dispatch path. Implementing either interface
/// produces an identical result — the mediator's <c>CreateStreamAsync</c> accepts both.
/// </para>
/// <para>
/// Prefer this type when working in stream-centric code that already imports
/// <c>Syntra.Syntra.Abstractions.Streams</c>. Prefer <see cref="Requests.IStreamQuery{TResponse}"/>
/// when working alongside other request types (<see cref="ICommand"/>, <see cref="IQuery{TResponse}"/>)
/// that all live in <c>Syntra.Syntra.Abstractions.Requests</c>.
/// </para>
/// </remarks>
public interface IStreamQuery<out TResponse> : Requests.IStreamQuery<TResponse> { }
