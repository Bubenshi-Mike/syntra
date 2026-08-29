namespace Syntra.Abstractions.Responses;

/// <summary>
/// Standard contract for paginated query responses.
/// Carries both the current page's items and the metadata required for
/// the client to navigate to other pages.
/// </summary>
/// <typeparam name="TItem">The type of item in the current page.</typeparam>
/// <remarks>
/// <para>
/// Use this interface as the <typeparamref name="TItem"/>-typed response for
/// paged queries:
/// <code>
/// public sealed record ListProductsQuery(int Page, int PageSize)
///     : IQuery&lt;IPagedResponse&lt;ProductDto&gt;&gt;;
/// </code>
/// </para>
/// <para>
/// <see cref="TotalPages"/> and the navigation flags are derived properties;
/// implement them as computed from <see cref="TotalCount"/> and <see cref="PageSize"/>.
/// </para>
/// </remarks>
public interface IPagedResponse<out TItem>
{
    /// <summary>Items in the current page.</summary>
    public IReadOnlyList<TItem> Items { get; }

    /// <summary>The 1-based index of the current page.</summary>
    public int Page { get; }

    /// <summary>The maximum number of items per page as requested by the caller.</summary>
    public int PageSize { get; }

    /// <summary>The total number of items across all pages.</summary>
    public int TotalCount { get; }

    /// <summary>The total number of pages, computed from <see cref="TotalCount"/> and <see cref="PageSize"/>.</summary>
    public int TotalPages { get; }

    /// <summary><c>true</c> when a page after the current one exists.</summary>
    public bool HasNextPage { get; }

    /// <summary><c>true</c> when a page before the current one exists.</summary>
    public bool HasPreviousPage { get; }
}
