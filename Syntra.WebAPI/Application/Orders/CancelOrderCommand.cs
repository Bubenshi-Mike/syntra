using Syntra.Abstractions.Guards;
using Syntra.Abstractions.Requests;

namespace Syntra.WebAPI.Application.Orders;

/// <summary>Cancellation requires ASP.NET Core policy <c>Orders.Cancel</c> (see Program.cs).</summary>
public sealed record CancelOrderCommand(Guid OrderId) : ICommand, IAuthorizedRequest
{
    public string? RequiredPolicy => "Orders.Cancel";
}
