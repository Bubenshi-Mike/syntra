using Syntra.Abstractions.Requests;
using Syntra.Behaviors.Retry;

namespace Syntra.ConsoleApp.Demo;

/// <summary>Handler throws until retries align — demonstrates retry behavior.</summary>
public sealed record FlakyEchoQuery(string Text) : IQuery<string>, IRetryableRequest;
