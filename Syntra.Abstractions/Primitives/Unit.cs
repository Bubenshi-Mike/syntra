using System.Runtime.InteropServices;

namespace Syntra.Abstractions.Primitives;

/// <summary>
/// Represents the absence of a meaningful return value.
/// Used as the type argument for <c>Result&lt;Unit&gt;</c> when a command succeeds
/// but has nothing to return — a type-safe replacement for <c>void</c> in
/// generic contexts.
/// </summary>
/// <remarks>
/// <para>
/// Because generic delegates and interfaces cannot be parameterised with <c>void</c>,
/// <see cref="Unit"/> fills that role:
/// <code>
/// Task&lt;Result&lt;Unit&gt;&gt; HandleAsync(MyCommand cmd, CancellationToken ct)
///     =&gt; Task.FromResult(Result.Success(Unit.Value));
/// </code>
/// </para>
/// <para>
/// <see cref="Unit"/> is a zero-byte struct; it incurs no allocation overhead.
/// All instances compare as equal.
/// </para>
/// </remarks>
[StructLayout(LayoutKind.Sequential, Size = 0)]
public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
{
    /// <summary>The singleton value. All instances are identical.</summary>
    public static readonly Unit Value = default;

    /// <inheritdoc />
    public bool Equals(Unit other) => true;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Unit;

    /// <inheritdoc />
    public override int GetHashCode() => 0;

    /// <inheritdoc />
    public int CompareTo(Unit other) => 0;

    /// <inheritdoc />
    public override string ToString() => "()";

    /// <summary>All <see cref="Unit"/> instances are equal.</summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>All <see cref="Unit"/> instances are equal; this always returns <c>false</c>.</summary>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Returns a completed <see cref="Task{TResult}"/> of <see cref="Unit"/>.
    /// Useful for satisfying async interfaces without allocating a new task.
    /// </summary>
    public static Task<Unit> Task { get; } = System.Threading.Tasks.Task.FromResult(Value);
}
