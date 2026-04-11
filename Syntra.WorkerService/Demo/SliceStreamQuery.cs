using Syntra.Abstractions.Requests;

namespace Syntra.WorkerService.Demo;

public sealed record SliceStreamQuery(int Count) : IStreamQuery<string>;
