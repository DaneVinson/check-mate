namespace CM.SignalR;

/// <summary>
/// SignalR hub through which domain events are pushed to authenticated clients.
/// </summary>
[Authorize]
public sealed class CheckMateHub : Hub
{
}
