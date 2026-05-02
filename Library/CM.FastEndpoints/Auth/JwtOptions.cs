namespace CM.FastEndpoints.Auth;

/// <summary>
/// Strongly-typed options for JWT token issuance and validation.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Gets or sets the intended audience of issued tokens.
    /// </summary>
    public string Audience { get; set; } = "checkmate-spa";

    /// <summary>
    /// Gets or sets the token lifetime in minutes.
    /// </summary>
    public int ExpiryMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = "https://checkmate.local";

    /// <summary>
    /// Gets or sets the HMAC-SHA256 signing key.
    /// </summary>
    public string SigningKey { get; set; } = string.Empty;
}
