namespace Syntra.Diagnostics.Tracing;

/// <summary>Semantic tags for Syntra spans.</summary>
public static class ActivityTagNames
{
    /// <summary>CLR request type name.</summary>
    public const string RequestType = "syntra.request.type";

    /// <summary>Whether the pipeline returned a successful <see cref="Results.Result"/>.</summary>
    public const string Success = "syntra.request.success";

    /// <summary>Primary error code when failed.</summary>
    public const string ErrorCode = "syntra.request.error_code";
}
