namespace CM.FastEndpoints.Auth;

/// <summary>
/// Signals logout via POST /auth/logout.
/// </summary>
/// <remarks>
/// JWT tokens are stateless; this endpoint does not invalidate the token server-side.
/// The caller is responsible for discarding the token after receiving a 204 response.
/// </remarks>
internal sealed class LogoutEndpoint : global::FastEndpoints.EndpointWithoutRequest
{
    /// <inheritdoc />
    public override void Configure()
    {
        Post("/auth/logout");
    }

    /// <inheritdoc />
    public override Task HandleAsync(CancellationToken ct)
    {
        HttpContext.Response.StatusCode = 204;
        return Task.CompletedTask;
    }
}
