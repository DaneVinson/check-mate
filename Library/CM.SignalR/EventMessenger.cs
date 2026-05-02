namespace CM.SignalR;

/// <summary>
/// <see cref="IMessenger{TMessage}"/> implementation that pushes domain events to the
/// authenticated user's SignalR connections via a single <c>ReceiveEvent</c> client method.
/// </summary>
internal sealed class EventMessenger : IMessenger<IEvent>
{
    private readonly IHubContext<CheckMateHub> _hubContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventMessenger"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor used to resolve the current user.</param>
    /// <param name="hubContext">The SignalR hub context for <see cref="CheckMateHub"/>.</param>
    public EventMessenger(IHttpContextAccessor httpContextAccessor, IHubContext<CheckMateHub> hubContext)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    /// <inheritdoc />
    public async Task SendAsync(IEvent message, CancellationToken cancellationToken = default)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
        {
            return;
        }

        await _hubContext.Clients.User(userId).SendAsync("ReceiveEvent", new
        {
            payload = message,
            type = message.GetType().Name,
        }, cancellationToken);
    }
}
