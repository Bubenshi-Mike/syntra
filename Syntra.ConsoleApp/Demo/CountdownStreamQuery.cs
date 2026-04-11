using Syntra.Abstractions.Requests;

namespace Syntra.ConsoleApp.Demo;

public sealed record CountdownStreamQuery(int Start, int End) : IStreamQuery<int>;
